using System.Diagnostics.CodeAnalysis;
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
        /// Initializes a new instance of the
        /// <see cref="BCryptPasswordHasher{TUser}"/> class.
        /// </summary>
        /// <param name="options"></param>
        public BCryptPasswordHasher(IOptions<BCryptPasswordHasherOptions> options)
        {
            _options = options?.Value ?? new BCryptPasswordHasherOptions();
        }

        #region IPasswordHasher<TUser> Members

        /// <inheritdoc />
        public string HashPassword(TUser user, string password)
        {
            Check.NotNull(user, nameof(user));
            return BCrypt.Net.BCrypt.HashPassword(password ?? string.Empty,
                _options.WorkFactor, _options.SaltRevision);
        }

        /// <inheritdoc />
        public PasswordVerificationResult VerifyHashedPassword(TUser user,
            string hashedPassword,
            string providedPassword)
        {
            Check.NotNull(user, nameof(user));
            Check.NotNullOrEmpty(hashedPassword, nameof(hashedPassword));
            Check.NotNullOrEmpty(providedPassword, nameof(providedPassword));

            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword)
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }

        #endregion

    }

}