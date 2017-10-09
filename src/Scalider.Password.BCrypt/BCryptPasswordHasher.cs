#region # using statements #

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

#endregion

namespace Scalider.Password
{

    /// <summary>
    /// Provides an implementation of the <see cref="IPasswordHasher"/> interface
    /// that uses BCrypt to hash and verify passwords.
    /// </summary>
    [UsedImplicitly,
     SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class BCryptPasswordHasher : IPasswordHasher
    {

        #region # Variables #

        private readonly BCryptPasswordHasherOptions _options;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BCryptPasswordHasher"/>
        /// class.
        /// </summary>
        public BCryptPasswordHasher()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BCryptPasswordHasher"/>
        /// class.
        /// </summary>
        /// <param name="options"></param>
        public BCryptPasswordHasher(IOptions<BCryptPasswordHasherOptions> options)
        {
            _options = options?.Value ?? new BCryptPasswordHasherOptions();
        }

        #region # IPasswordHasher #

        /// <inheritdoc />
        public virtual string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password ?? string.Empty,
                _options.WorkFactor);

        /// <inheritdoc />
        public Task<string> HashPasswordAsync(string password) =>
            HashPasswordAsync(password, CancellationToken.None);

        /// <inheritdoc />
        public virtual Task<string> HashPasswordAsync(string password,
            CancellationToken cancellationToken) =>
            Task.FromResult(HashPassword(password));

        /// <inheritdoc />
        public virtual PasswordVerificationResult VerifyPasswordHash(
            string hashedPassword, string providedPassword)
        {
            Check.NotNullOrEmpty(hashedPassword, nameof(hashedPassword));
            Check.NotNullOrEmpty(providedPassword, nameof(providedPassword));

            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword)
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }

        /// <inheritdoc />
        public Task<PasswordVerificationResult>
            VerifyPasswordHashAsync(string hashedPassword,
                string providedPassword) => VerifyPasswordHashAsync(hashedPassword,
            providedPassword, CancellationToken.None);

        /// <inheritdoc />
        public virtual Task<PasswordVerificationResult> VerifyPasswordHashAsync(
            string hashedPassword, string providedPassword,
            CancellationToken cancellationToken) =>
            Task.FromResult(VerifyPasswordHash(hashedPassword, providedPassword));

        #endregion

    }

}