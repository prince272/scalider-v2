using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace Scalider
{

    /// <summary>
    /// Provides utility methods for calculating and validating the LUHN algorithm.
    /// </summary>
    public static class LuhnUtility
    {

        private static readonly int[] Mod9Weights = {7, 9, 8, 6, 5, 4, 3, 2};

        /// <summary>
        /// <para>
        /// Calculates the corresponding validator character for the given <paramref name="input" /> using the
        /// LUHN algorithm.
        /// </para>
        /// <para>
        /// This will only take into consideration the digits (0-9) and will ignore anything outside of that range.
        /// </para>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <example>
        ///     CalculateMod10("123454") returns the character '1' 
        ///     CalculateMod10("4a1b3cd254") returns the character '4' 
        ///     CalculateMod10("abcd") returns the character '0' 
        /// </example>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static char CalculateMod10([NotNull] string input)
        {
            Check.NotNullOrEmpty(input, nameof(input));

            // Append a zero at the end of the input so we can apply the right
            // weight to every character in the string
            input += "0";

            // Calculate the sum of all the digits on the string
            var sumOfDigits =
                input.Where(char.IsDigit)
                     .Reverse()
                     .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                     .Sum(e => e / 10 + e % 10);

            // Done
            var checkDigit = sumOfDigits % 10;
            return (char)((checkDigit == 0 ? checkDigit : 10 - checkDigit) + '0');
        }

        /// <summary>
        /// Validates the given <paramref name="input"/> using the LUHN algorithm.
        /// </summary>
        /// <param name="input">The string to validate.</param>
        /// <returns>
        /// <c>true</c> if the given <paramref name="input"/> is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMod10(string input) =>
            !string.IsNullOrEmpty(input) && input.Last() == CalculateMod10(input.Substring(0, input.Length - 1));

        /// <summary>
        /// Validates the given <paramref name="input"/> using a fixed weight for the LUHN algorithm.
        /// </summary>
        /// <param name="input">The string to validate.</param>
        /// <returns>
        /// <c>true</c> if the given <paramref name="input"/> is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMod9(string input)
        {
            var numInput = new string((input ?? "").Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(numInput))
                return false;

            // Calculate the checksum for the string
            var checksum = 0;
            for (var i = numInput.Length - 2; i >= 0; i--)
                checksum += (numInput[i] - '0') * Mod9Weights[i];

            checksum %= 11;
            var validator = checksum == 0 ? 2 : checksum == 1 ? 1 : 11 - checksum;

            // Done
            return validator == int.Parse(numInput.Substring(numInput.Length - 1));
        }

    }
}