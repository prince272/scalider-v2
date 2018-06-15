using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Net.Mail
{
    
    /// <summary>
    /// Defines the basic functionality of an email address validator.
    /// </summary>
    public interface IEmailAddressValidator
    {

        /// <summary>
        /// Determines whether the given <paramref name="value"/> is a valid email address.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="value" /> is a valid email address; otherwise <c>false</c>.
        /// </returns>
        [UsedImplicitly]
        bool IsValid([NotNull] string value);

        /// <summary>
        /// Asynchronously determines whether the given <paramref name="value"/> is a valid email address.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task<bool> IsValidAsync([NotNull] string value,
            [UsedImplicitly] CancellationToken cancellationToken = default);

    }
    
}