#region # using statements #

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        /// Validates that <paramref name="value"/> is not NaN or infinity and throws
        /// an exception when this condition is not met.
        /// </summary>
        /// <param name="value">The number to validate.</param>
        /// <param name="paramName">The name of the argument to use to report as
        /// null.</param>
        /// <exception cref="ArgumentException">When <paramref name="value"/>
        /// is NaN or infinity.</exception>
        public static void IsValidNumber(float value,
            [InvokerParameterName, NotNull] string paramName)
        {
            NameIsNotNullOrEmpty(paramName, nameof(paramName));
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException(
                    "The value must be a valid finite number", paramName);
            }
        }

        /// <summary>
        /// Validates that <paramref name="value"/> is not NaN or infinity and throws
        /// an exception when this condition is not met.
        /// </summary>
        /// <param name="value">The number to validate.</param>
        /// <param name="paramName">The name of the argument to use to report as
        /// null.</param>
        /// <exception cref="ArgumentException">When <paramref name="value"/>
        /// is NaN or infinity.</exception>
        public static void IsValidNumber(double value,
            [InvokerParameterName, NotNull] string paramName)
        {
            NameIsNotNullOrEmpty(paramName, nameof(paramName));
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentNullException(
                    "The value must be a valid finite number", paramName);
            }
        }

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

        /// <summary>
        /// Validates that <paramref name="value"/> is not null or only contains
        /// whitespaces and throws an exception if the condition is not met.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        /// <exception cref="ArgumentNullException">When <paramref name="value"/>
        /// is null.</exception>
        /// <exception cref="ArgumentException">When <paramref name="value"/> only
        /// contains whitespaces.</exception>
        [ContractAnnotation("value:null => halt")]
        public static void NotNullOrEmpty(string value,
            [InvokerParameterName, NotNull] string paramName)
        {
            NameIsNotNullOrEmpty(paramName, nameof(paramName));
            if (value == null)
                throw new ArgumentNullException(paramName);
            
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    $"The string parameter {paramName} cannot be null or " +
                    "contain only whitespaces.", paramName);
            }
        }

        public static void IsValidFileName(string value,
            [InvokerParameterName, NotNull] string paramName)
        {
            NotNullOrEmpty(value, nameof(value));
            NameIsNotNullOrEmpty(paramName, nameof(paramName));

            if (value.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                throw new ArgumentException(
                    "The file name contains invalid characters", paramName);
            }
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