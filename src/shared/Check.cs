#region # using statements #

using System;
using System.Diagnostics;
using JetBrains.Annotations;

#endregion

namespace Scalider
{

    /// <summary>
    /// Provide helper methods for throwing exceptions when certain conditions are
    /// met.
    /// </summary>
    [DebuggerStepThrough]
    internal static class Check
    {

        /// <summary>
        /// Validates that <paramref name="value"/> is not null and throws an
        /// exception if this condition is not met.
        /// </summary>
        /// <param name="value">The value to check for null.</param>
        /// <param name="paramName">The name of the argument to use to report as
        /// null.</param>
        /// <typeparam name="T">The type encapsulating the value.</typeparam>
        /// <exception cref="ArgumentNullException">When <paramref name="value"/>
        /// is null.</exception>
        [ContractAnnotation("value:null => halt")]
        public static void NotNull<T>([NoEnumeration] T value,
            [InvokerParameterName, NotNull] string paramName)
        {
            NameIsNotNullOrEmpty(paramName, nameof(paramName));
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        private static void NameIsNotNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(
                    "The name should not be null or an empty string",
                    name
                );
            }
        }

    }

}