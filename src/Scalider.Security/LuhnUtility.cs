using System;
using System.Linq;
using JetBrains.Annotations;

namespace Scalider
{

    /// <summary>
    /// Provides utility methods for the LUHN algorithm.
    /// </summary>
    public static class LuhnUtility
    {

        /// <summary>
        /// Calculates the LUHN check digit of the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The value to calculate the LUHN check digit for.</param>
        /// <returns>
        /// A digit representing the check digit of the given <paramref name="input"/>.
        /// </returns>
        /// <exception cref="System.ArgumentException">When <paramref name="input"/> is empty or contains a
        /// non-numeric character.</exception>
        /// <exception cref="System.ArgumentNullException">When <paramref name="input"/> is <c>NULL</c>.</exception>
        [UsedImplicitly]
        public static int CalculateCheckDigit([NotNull] string input)
        {
            Check.NotNullOrEmpty(input, nameof(input));

            // Determine if the given input contains only digits
            var inputDigits = input.Where(char.IsDigit).ToArray();
            if (inputDigits.Length != input.Length)
            {
                // There was at least one non-numeric character
                throw new ArgumentException(
                    "The value must be a string containing only digits.",
                    nameof(input)
                );
            }

            // Append a zero at the end of the input so we can apply the right
            // weight to every character in the string
            inputDigits = inputDigits.Append('0').ToArray();

            // Calculate the sum of all the digits on the string
            var sumOfDigits = inputDigits
                              .Reverse()
                              .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                              .Sum(e => e / 10 + e % 10);

            // Done
            var checkDigit = sumOfDigits % 10;
            // return (char)((checkDigit == 0 ? checkDigit : 10 - checkDigit) + '0');
            return checkDigit == 0 ? checkDigit : 10 - checkDigit;
        }

        /// <summary>
        /// Determines wether the last character of the given <paramref name="input"/> is the valid
        /// LUHN check digit for the value.
        /// </summary>
        /// <param name="input">The value to validate.</param>
        /// <returns>
        /// <c>true</c> if the last character of <paramref name="input"/> is the valid LUHN check digit; otherwise,
        /// <c>false</c>.
        /// </returns>
        [UsedImplicitly]
        public static bool Validate(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // Remove special characters from the input string
            input = input.Replace(" ", "")
                         .Replace("-", "")
                         .Replace("_", "")
                         .Replace("*", "");
            
            // Determine if the last character of the given input is a digit
            var lastCharacter = input.Last();
            if (!int.TryParse(lastCharacter.ToString(), out var lastDigit))
                return false;

            // Done
            return lastDigit == CalculateCheckDigit(input.Substring(0, input.Length - 1));
        }

    }
}