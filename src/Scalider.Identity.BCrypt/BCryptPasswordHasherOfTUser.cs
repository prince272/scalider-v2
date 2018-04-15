using System.Diagnostics.CodeAnalysis;
using BCrypt.Net;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Scalider.Identity
{

    /// <summary>
    /// Provides an implementation of the <see cref="IPasswordHasher{TUser}"/>
    /// interface that uses BCrypt to hash and verify passwords.
    /// </summary>
    /// <typeparam name="TUser">The type used to represent a user.</typeparam>
    [UsedImplicitly,
     SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class BCryptPasswordHasher<TUser> : IPasswordHasher<TUser>
        where TUser : class
    {

        private readonly BCryptPasswordHasherOptions _options;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BCryptPasswordHasher{TUser}"/> class.
        /// </summary>
        public BCryptPasswordHasher()
        {
            _options = new BCryptPasswordHasherOptions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BCryptPasswordHasher{TUser}"/> class.
        /// </summary>
        /// <param name="options"></param>
        public BCryptPasswordHasher(IOptions<BCryptPasswordHasherOptions> options)
        {
            _options = options?.Value ?? new BCryptPasswordHasherOptions();
        }

        private static bool IsSameSaltRevision(string revision,
            SaltRevision expectedRevision)
        {
            if (revision.Length != 2)
                return expectedRevision == SaltRevision.Revision2;

            // Validate the salt revision suffix
            switch (revision[1])
            {
                case 'a':
                    return expectedRevision == SaltRevision.Revision2A;
                case 'b':
                    return expectedRevision == SaltRevision.Revision2B;
                case 'x':
                    return expectedRevision == SaltRevision.Revision2X;
                case 'y':
                    return expectedRevision == SaltRevision.Revision2Y;
            }

            // Unknown salt revision
            return false;
        }

        #region IPasswordHasher<TUser> Members

        /// <inheritdoc />
        public string HashPassword(TUser user, string password)
        {
            Check.NotNull(user, nameof(user));
            return BCrypt.Net.BCrypt.HashPassword(password ?? string.Empty, _options.WorkFactor, _options.SaltRevision);
        }

        /// <inheritdoc />
        public PasswordVerificationResult VerifyHashedPassword(TUser user,
            string hashedPassword, string providedPassword)
        {
            Check.NotNull(user, nameof(user));
            Check.NotNullOrEmpty(hashedPassword, nameof(hashedPassword));
            Check.NotNullOrEmpty(providedPassword, nameof(providedPassword));

            // Determine if the hashedPassword is valid
            var hashInfo = BCryptPasswordHasherOptions.HashInformation.Match(hashedPassword);
            if (!hashInfo.Success)
            {
                // The hashed password is invalid
                return PasswordVerificationResult.Failed;
            }

            // Verify the password
            var verifyResult = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
            if (!verifyResult)
            {
                // The password is invalid
                return PasswordVerificationResult.Failed;
            }

            // Retrieve the salt information
            //            var saltInfo =
            //                BCryptPasswordHasherOptions.SettingsInformation.Match(
            //                    hashInfo.Groups["settings"].Value);
            //
            //            if (!saltInfo.Success)
            //            {
            //                // Could not retrieve the version of the hash
            //                return PasswordVerificationResult.Success;
            //            }

            // Determine if the password needs rehashing
            if (int.TryParse(hashInfo.Groups["rounds"].Value, out var rounds) &&
                (rounds != _options.WorkFactor ||
                 !IsSameSaltRevision(hashInfo.Groups["revision"].Value, _options.SaltRevision)))
            {
                // The hashed password needs rehashing
                return PasswordVerificationResult.SuccessRehashNeeded;
            }

            // Done
            return PasswordVerificationResult.Success;
        }

        #endregion

    }

}