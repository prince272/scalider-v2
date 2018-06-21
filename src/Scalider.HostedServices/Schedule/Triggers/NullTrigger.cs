using System;

namespace Scalider.Hosting.Schedule.Triggers
{

    internal class NullTrigger : AbstractTrigger
    {

        public static readonly NullTrigger Instance = new NullTrigger();

        /// <inheritdoc />
        public override bool ShouldRemoveTask => true;

        /// <inheritdoc />
        public override DateTimeOffset? GetExecutionTimeAfter(DateTimeOffset utcNow, int executionCount) => null;

    }

}