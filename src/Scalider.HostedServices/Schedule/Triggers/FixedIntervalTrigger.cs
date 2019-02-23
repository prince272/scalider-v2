using System;
using JetBrains.Annotations;

namespace Scalider.Hosting.Schedule.Triggers
{

    /// <summary>
    /// Provides an implementation of the <see cref="ITrigger"/> that uses a fixed interval to calculate
    /// the execution time for <see cref="ISchedulableTask"/>s.
    /// </summary>
    public class FixedIntervalTrigger : AbstractTrigger
    {

        /// <summary>
        /// Repeat the task indefinitely.
        /// </summary>
        [UsedImplicitly]
        public const int RepeatIndefinitely = -1;

        private int _repeatCount = RepeatIndefinitely;
        private bool _shouldRemoveTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIntervalTrigger"/> class.
        /// </summary>
        /// <param name="repeatInterval"></param>
        public FixedIntervalTrigger(TimeSpan repeatInterval)
        {
            if (repeatInterval < TimeSpan.Zero)
            {
            }

            RepeatInterval = repeatInterval;
        }

        /// <summary>
        /// Gets a <see cref="FixedIntervalTrigger"/> that schedules a task for every minute.
        /// </summary>
        public static FixedIntervalTrigger Minutely => new FixedIntervalTrigger(TimeSpan.FromMinutes(1));

        /// <summary>
        /// Gets a <see cref="FixedIntervalTrigger"/> that schedules a task for every hour.
        /// </summary>
        public static FixedIntervalTrigger Hourly => new FixedIntervalTrigger(TimeSpan.FromHours(1));

        /// <summary>
        /// Gets a <see cref="FixedIntervalTrigger"/> that schedules a task for every day.
        /// </summary>
        public static FixedIntervalTrigger Daily => new FixedIntervalTrigger(TimeSpan.FromDays(1));

        /// <summary>
        /// Gets a <see cref="FixedIntervalTrigger"/> that schedules a task for every 7 days.
        /// </summary>
        public static FixedIntervalTrigger Weekly => new FixedIntervalTrigger(TimeSpan.FromDays(7));

        /// <inheritdoc />
        public override bool ShouldRemoveTask => _shouldRemoveTask;

        /// <summary>
        /// Gets or sets a value indicating the total number of times the interval should repeat.
        /// </summary>
        [UsedImplicitly]
        public int RepeatCount
        {
            get => _repeatCount;
            set
            {
                if (value < 0 && value != RepeatIndefinitely)
                {
                    throw new ArgumentException(
                        $"Repeat count must be >= 0, use the constant {nameof(RepeatIndefinitely)} for infinite.",
                        nameof(value)
                    );
                }

                _repeatCount = value;
            }
        }

        /// <summary>
        /// Gets a value indicating the repeat interval for the task.
        /// </summary>
        [UsedImplicitly]
        public TimeSpan RepeatInterval { get; }

        /// <inheritdoc />
        public override DateTimeOffset? GetExecutionTimeAfter(DateTimeOffset utcNow, int executionCount)
        {
            if (RepeatCount != RepeatIndefinitely && executionCount > RepeatCount - 1)
            {
                // No more possible execution when reaching the maximum repeat count
                _shouldRemoveTask = true;
                return null;
            }

            // Calculate the lower and upper execution bounds
            var lowerBound = StartTimeUtc?.DateTime ?? DateTime.MinValue.ToUniversalTime();
            var upperBound = EndTimeUtc?.DateTime ?? DateTime.MaxValue.ToUniversalTime();

            utcNow = lowerBound > utcNow ? lowerBound : utcNow;
            if (upperBound < utcNow)
            {
                // The given date and time is outside the execution range for the task
                return null;
            }

            // Try to retrieve the next execution time
            try
            {
                return utcNow.Add(RepeatInterval);
            }
            catch
            {
                // ignore
            }

            // It was not possible to determine the next execution time
            return null;
        }

    }

}