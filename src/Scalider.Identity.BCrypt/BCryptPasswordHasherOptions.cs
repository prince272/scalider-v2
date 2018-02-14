using System;
using BCrypt.Net;
using JetBrains.Annotations;

namespace Scalider.Identity
{

    /// <summary>
    /// Options used for <see cref="BCryptPasswordHasher{TUser}"/>.
    /// </summary>
    public class BCryptPasswordHasherOptions
    {

        /// <summary>
        /// The default work factor.
        /// </summary>
        [UsedImplicitly]
        public const int DefaultWorkFactor = 10;

        /// <summary>
        /// Gets a value indicating the minimum work factor allowed.
        /// </summary>
        [UsedImplicitly]
        public const int MinimumAllowedWorkFactor = 4;
        
        /// <summary>
        /// Gets a value indicating the maximum work factor allowed.
        /// </summary>
        [UsedImplicitly]
        public const int MaximumAllowedWorkFactor = 31;

        private int _workFactor = DefaultWorkFactor;

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
        [UsedImplicitly]
        public int WorkFactor
        {
            get => _workFactor;
            set
            {
                if (value < MinimumAllowedWorkFactor ||
                    value > MaximumAllowedWorkFactor)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        "The work factor must be between " +
                        $"${MinimumAllowedWorkFactor} and " +
                        $"${MaximumAllowedWorkFactor} (inclusive)"
                    );
                }

                _workFactor = value;
            }
        }

        /// <summary>
        /// Gets or sets the version of the salt.
        /// </summary>
        public SaltRevision SaltRevision { get; set; } = SaltRevision.Revision2B;

    }
}