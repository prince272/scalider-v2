using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Mail
{

    /// <summary>
    /// Represents an implementation of the <see cref="IEmailAddressValidator"/> that allows international email
    /// addresses.
    /// </summary>
    /// <remarks>
    /// Inspired by
    /// https://github.com/JeremySkinner/FluentValidation/blob/master/src/FluentValidation/Validators/EmailValidator.cs
    /// https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
    /// </remarks>
    [UsedImplicitly,
     SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class EmailAddressValidator : IEmailAddressValidator
    {

        private readonly IdnMapping _idnMapping;
        private readonly Regex _domainMapperRegex;
        private readonly Regex _validatorRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressValidator"/> class/
        /// </summary>
        public EmailAddressValidator()
        {
            _idnMapping = new IdnMapping();
            _domainMapperRegex = new Regex(
                @"(@)(.+)$",
                RegexOptions.None,
                TimeSpan.FromMilliseconds(50)
            );

            _validatorRegex = CreateValidatorRegex();
        }

        /// <summary>
        /// Determines whether the given domain name is allowed.
        /// </summary>
        /// <param name="domainName">The domain name to validate.</param>
        /// <returns>
        /// <c>true</c> if the domain name is allowed; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsDomainNameAllowed([NotNull] string domainName) =>
            !string.IsNullOrWhiteSpace(domainName);

        private static Regex CreateValidatorRegex() => new Regex(
            @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|
[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|
[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|
((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|
[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|
[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))
@
(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace,
            TimeSpan.FromMilliseconds(200)
        );

        #region IEmailAddressValidator Members

        /// <inheritdoc />
        public virtual bool IsValid([CanBeNull] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                // An empty email address will alweays be considered invalid
                return false;
            }

            // Use IdnMapping class to convert Unicode domain names
            string realEmailAddress;
            var state = new ValidationState(_idnMapping);

            try
            {
                realEmailAddress = _domainMapperRegex.Replace(value, state.DomainMapper);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            // Determine if the email address domain name is valid
            if (!state.IsValidDomainName || !IsDomainNameAllowed(state.DomainName))
            {
                // The domain name for the email address isn't valid or isn't allowed
                return false;
            }

            // Use Regex to determine whether the email address has a valid format
            try
            {
                return _validatorRegex.IsMatch(realEmailAddress);
            }
            catch (RegexMatchTimeoutException)
            {
            }

            // Could not validate the email address
            return false;
        }

        /// <inheritdoc />
        public Task<bool> IsValidAsync(string value, CancellationToken cancellationToken = default) =>
            Task.FromResult(IsValid(value));

        #endregion

        #region Nested type: ValidationState

        private class ValidationState
        {
            
            private static readonly Regex IsInternationalAddressRegex;

            private readonly IdnMapping _idnMapping;

            static ValidationState()
            {
                IsInternationalAddressRegex = new Regex(@"[^\u0000-\u007F]", RegexOptions.None);
            }

            public ValidationState(IdnMapping idnMapping)
            {
                _idnMapping = idnMapping;
            }

            public bool IsValidDomainName { get; private set; }

            public string DomainName { get; private set; }

            public string DomainMapper(Match match)
            {
                var domainName = match.Groups[2].Value;
                if (IsInternationalAddressRegex.IsMatch(domainName))
                {
                    // Only try to map the IDN for international domains
                    try
                    {
                        domainName = _idnMapping.GetAscii(domainName);
                        IsValidDomainName = true; // We could convert the domain, assume is valid
                    }
                    catch (ArgumentException)
                    {
                        IsValidDomainName = false;
                    }
                }
                else
                {
                    // Otherwise, we assume the domain name is valid
                    IsValidDomainName = true;
                }

                // Done
                DomainName = domainName;
                return match.Groups[1].Value + domainName;
            }

        }

        #endregion

    }

}