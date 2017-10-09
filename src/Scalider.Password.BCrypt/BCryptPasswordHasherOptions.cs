#region # using statements #

using JetBrains.Annotations;

#endregion

namespace Scalider.Password
{
    
    /// <summary>
    /// Options used for <see cref="BCryptPasswordHasher"/>.
    /// </summary>
    public class BCryptPasswordHasherOptions
    {

        #region # Constants #

        /// <summary>
        /// The default work factor.
        /// </summary>
        [UsedImplicitly] public const int DefaultWorkFactor = 10;

        #endregion

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BCryptPasswordHasherOptions"/> class.
        /// </summary>
        public BCryptPasswordHasherOptions()
        {
        }

        #region # Properties #

        #region == Public ==

        /// <summary>
        /// Gets the log2 of the number of rounds of hashing to apply - the
        /// work factor therefore increases as 2^WorkFactor.
        /// </summary>
        public int WorkFactor { get; [UsedImplicitly] set; } = DefaultWorkFactor;

        #endregion

        #endregion

    }
}