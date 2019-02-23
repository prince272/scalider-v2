using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CronExpressionDescriptor;
using JetBrains.Annotations;
using NCrontab;

namespace Scalider.Hosting.Schedule.Triggers
{

    /// <summary>
    /// Provides an implementation of the <see cref="ITrigger"/> interface that uses crontab expressions
    /// to calculate the execution time for <see cref="ISchedulableTask"/>s.
    ///
    /// Multiple expressions can be separated using the semicolon (;).
    /// </summary>
    /// <code>
    /// Example of crontab expression:
    ///
    /// * * * * *
    /// - - - - -
    /// | | | | |
    /// | | | | +----- day of week (0 - 6) (Sunday=0)
    /// | | | +------- month (1 - 12)
    /// | | +--------- day of month (1 - 31)
    /// | +----------- hour (0 - 23)
    /// +------------- min (0 - 59)
    /// </code>
    /// <remarks>
    /// https://github.com/atifaziz/NCrontab/wiki/Crontab-Expression
    /// </remarks>
    [UsedImplicitly]
    [DebuggerDisplay("{Description}")]
    public class CrontabTrigger : AbstractTrigger
    {

        private static readonly Options ExpressionParserOptions = new Options();

        private readonly string _cronExpression;
        private readonly CrontabSchedule[] _schedules;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrontabTrigger"/> class.
        /// </summary>
        /// <param name="expression"></param>
        public CrontabTrigger([NotNull] string expression)
        {
            Check.NotNullOrEmpty(expression, nameof(expression));

            _cronExpression = expression;

            // Parse all the possible expressions
            var expressions = expression.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                                        .Where(t => !string.IsNullOrWhiteSpace(t))
                                        .Select(NormalizeExpression)
                                        .ToArray();

            _schedules = expressions.Length > 1
                ? expressions.Select(CrontabSchedule.Parse).ToArray()
                : new[] {CrontabSchedule.Parse(expressions.First())};

            // Retrieve the description for the given expression
            Description = string.Join(
                "; ",
                expressions.Select(t => ExpressionDescriptor.GetDescription(t, ExpressionParserOptions))
            );
        }

        private static string NormalizeExpression(string expression)
        {
            var expressionParser = new ExpressionParser(expression, ExpressionParserOptions);
            var parsedExpression = expressionParser.Parse();
            var expressionParts = parsedExpression.Skip(1).Take(5);

            return string.Join(" ", expressionParts);
        }

        /// <summary>
        /// Gets a textual representation of the crontab expression.
        /// </summary>
        [UsedImplicitly]
        public string Description { get; }

        /// <summary>
        /// Gets a <see cref="CrontabTrigger"/> that schedules a task for every minute.
        /// </summary>
        [UsedImplicitly]
        public static CrontabTrigger Minutely => new CrontabTrigger("* * * * *");

        /// <summary>
        /// Gets a <see cref="CrontabTrigger"/> that schedules a task for every hour.
        /// </summary>
        [UsedImplicitly]
        public static CrontabTrigger Hourly => new CrontabTrigger("0 */1 * * *");

        /// <summary>
        /// Gets a <see cref="CrontabTrigger"/> that schedules a task for every day.
        /// </summary>
        [UsedImplicitly]
        public static CrontabTrigger Daily => new CrontabTrigger("0 0 */1 * *");

        /// <summary>
        /// Gets a <see cref="CrontabTrigger"/> that schedules a task for every 7 days.
        /// </summary>
        [UsedImplicitly]
        public static CrontabTrigger Weekly => new CrontabTrigger("0 0 */7 * *");

        /// <summary>
        /// Gets a <see cref="CrontabTrigger"/> that schedules a task for the first day of every month.
        /// </summary>
        [UsedImplicitly]
        public static CrontabTrigger Monthly => new CrontabTrigger("0 0 1 */1 *");

        /// <inheritdoc />
        public override bool ShouldRemoveTask => false;

        /// <inheritdoc />
        public override string ToString() => $"{_cronExpression} ({Description})";

        /// <inheritdoc />
        public override DateTimeOffset? GetExecutionTimeAfter(DateTimeOffset utcNow, int executionCount)
        {
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
                if (_schedules.Length == 1)
                {
                    var singleResult = _schedules[0].GetNextOccurrence(utcNow.DateTime, upperBound);
                    return new DateTimeOffset(singleResult, TimeSpan.Zero);
                }

                var result = Merge(utcNow.DateTime, upperBound);
                if (result.Length > 0)
                    return result[0];

                return null;
            }
            catch
            {
                // ignore
            }

            // It was not possible to determine the next execution time
            return null;
        }

        private static KeyValuePair<TKey, TValue> Pair<TKey, TValue>(TKey key, TValue value) =>
            new KeyValuePair<TKey, TValue>(key, value);

        private DateTimeOffset[] Merge(DateTime utcNow, DateTime endTimeUtc)
        {
            return Merge(
                    _schedules,
                    s => new[]{s.GetNextOccurrence(utcNow, endTimeUtc)},
                    (_, dt) => new DateTimeOffset(dt, TimeSpan.Zero)
                )
                .ToArray();
        }

        private static IEnumerable<TResult> Merge<T, TSortable, TResult>(IEnumerable<T> sources,
            Func<T, IEnumerable<TSortable>> sortablesSelector, Func<T, TSortable, TResult> resultSelector)
            where TSortable : IComparable<TSortable>
        {
            var enumerators = new List<KeyValuePair<T, IEnumerator<TSortable>>>();

            try
            {
                enumerators.AddRange(from t in sources
                                     select Pair(t, sortablesSelector(t).GetEnumerator()));
            }
            catch
            {
                foreach (var e in enumerators)
                    e.Value.Dispose();
                throw;
            }

            try
            {
                for (var i = enumerators.Count - 1; i >= 0; i--)
                    if (!enumerators[i].Value.MoveNext())
                    {
                        enumerators[i].Value.Dispose();
                        enumerators.RemoveAt(i);
                    }

                while (enumerators.Count > 0)
                {
                    var i = 0;
                    var e = enumerators[0];

                    for (var xi = 1; xi < enumerators.Count; xi++)
                    {
                        var xe = enumerators[xi];
                        var comparison = xe.Value.Current?.CompareTo(e.Value.Current);
                        if (comparison < 0)
                        {
                            i = xi;
                            e = xe;
                        }
                        else if (comparison == 0)
                        {
                            i = xi;
                            goto skip;
                        }
                    }

                    yield return resultSelector(e.Key, e.Value.Current);

                    skip:

                    // advance iterator that yielded element, excluding it when consumed
                    if (!enumerators[i].Value.MoveNext())
                    {
                        enumerators[i].Value.Dispose();
                        enumerators.RemoveAt(i);
                    }
                }
            }
            finally
            {
                foreach (var e in enumerators)
                    e.Value.Dispose();
            }
        }

    }

}