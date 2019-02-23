using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Scalider.Hosting.Schedule.Triggers
{

    /// <summary>
    /// Provides a base implementation of the <see cref="ITrigger"/> interface.
    /// </summary>
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
    public abstract class AbstractTrigger : ITrigger
    {

        /// <summary>
        /// Gets or sets a value indicating the minimum allowed date and time (with timezone UTC) for the
        /// scheduled task.
        /// </summary>
        [UsedImplicitly]
        public virtual DateTimeOffset? StartTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the maximum allowed date and time (with timezone UTC) for the
        /// scheduled task.
        /// </summary>
        [UsedImplicitly]
        public virtual DateTimeOffset? EndTimeUtc { get; }

        /// <inheritdoc />
        public abstract bool ShouldRemoveTask { get; }

        /// <inheritdoc />
        public abstract DateTimeOffset? GetExecutionTimeAfter(DateTimeOffset utcNow, int executionCount);

    }

}