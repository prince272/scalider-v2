using JetBrains.Annotations;

namespace Scalider.Password
{

    /// <summary>
    /// Options used for <see cref="BCryptPasswordHasher{TUser}"/>.
    /// </summary>
    public class BCryptPasswordHasherOptions
    {

        /// <summary>
        /// The default work factor.
        /// </summary>
        [UsedImplicitly] public const int DefaultWorkFactor = 10;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BCryptPasswordHasherOptions"/> class.
        /// </summary>
        public BCryptPasswordHasherOptions()
        {
        }

        /// <summary>
        /// Gets the log2 of the number of rounds of hashing to apply - the
        /// work factor therefore increases as 2^WorkFactor.
        /// </summary>
        public int WorkFactor { get; [UsedImplicitly] set; } = DefaultWorkFactor;

    }
}