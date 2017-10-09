#region # using statements #

using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Scalider.Password
{

    /// <summary>
    /// A password hasher represent the strategy used to hash and verify a password.
    /// </summary>
    public interface IPasswordHasher
    {

        /// <summary>
        /// Returns the hash representation on the provided
        /// <paramref name="password"/>.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>
        /// A hashed representation of the supplied
        /// <paramref name="password"/>.
        /// </returns>
        string HashPassword(string password);

        /// <summary>
        /// Asynchronously returns the hash representation on the provided
        /// <paramref name="password"/>.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<string> HashPasswordAsync(string password);

        /// <summary>
        /// Asynchronously returns the hash representation on the provided
        /// <paramref name="password"/>.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<string> HashPasswordAsync(string password,
            CancellationToken cancellationToken);

        /// <summary>
        /// Returns a <see cref="PasswordVerificationResult"/> indicating the
        /// result of a password hash comparison.
        /// </summary>
        /// <param name="hashedPassword">The hash value for a user's stored
        /// password.</param>
        /// <param name="providedPassword">The password supplied for
        /// comparison.</param>
        /// <returns>
        /// A <see cref="PasswordVerificationResult"/> indicating the result of
        /// a password hash comparison.
        /// </returns>
        /// <remarks>
        /// Implementations of this method should be time consistent.
        /// </remarks>
        PasswordVerificationResult VerifyPasswordHash(string hashedPassword,
            string providedPassword);

        /// <summary>
        /// Asynchronously eturns a <see cref="PasswordVerificationResult"/>
        /// indicating the result of a password hash comparison.
        /// </summary>
        /// <param name="hashedPassword">The hash value for a user's stored
        /// password.</param>
        /// <param name="providedPassword">The password supplied for
        /// comparison.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<PasswordVerificationResult> VerifyPasswordHashAsync(
            string hashedPassword, string providedPassword);

        /// <summary>
        /// Asynchronously eturns a <see cref="PasswordVerificationResult"/>
        /// indicating the result of a password hash comparison.
        /// </summary>
        /// <param name="hashedPassword">The hash value for a user's stored
        /// password.</param>
        /// <param name="providedPassword">The password supplied for
        /// comparison.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<PasswordVerificationResult> VerifyPasswordHashAsync(
            string hashedPassword, string providedPassword,
            CancellationToken cancellationToken);

    }
}