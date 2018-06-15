using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Scalider
{
    
    /// <summary>
    /// Provides extension methods for the <see cref="string"/> class for handling normalization.
    /// </summary>
    [UsedImplicitly]
    public static class GlobalizationStringExtensions
    {

        /// <summary>
        /// Replaces the non-ascii characters with the encoded representation of the same character.
        /// </summary>
        /// <param name="str">The string to encode.</param>
        /// <returns>
        /// The string representation with the replaced non-ascii characters.
        /// </returns>
        /// <remarks>
        /// See http://stackoverflow.com/a/1615860/2411798
        /// </remarks>
        public static string EncodeNonAsciiCharacters(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var chr in str)
            {
                if (chr > 127)
                    sb.Append("\\u" + ((int)chr).ToString("x4"));
                else
                    sb.Append(chr);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Replaces the encoded representation of non-ascii characters with the ascii representation of the same
        /// character.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <returns>
        /// The string representation with the replaced ascii characters.
        /// </returns>
        /// <remarks>
        /// See http://stackoverflow.com/a/1615860/2411798
        /// </remarks>
        public static string DecodeNonAsciiCharacters(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            return Regex.Replace(
                str,
                @"\\u(?<Value>[a-z0-9]{4})",
                m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString(),
                RegexOptions.IgnoreCase
            );
        }

        /// <summary>
        /// Removes the diacritics from a given string.
        /// </summary>
        /// <param name="str">The string to remove diacritics from.</param>
        /// <returns>
        /// The string without diacritics.
        /// </returns>
        public static string RemoveDiacritics(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            var normalized = from chr in str.Normalize(NormalizationForm.FormD)
                             let cui = CharUnicodeInfo.GetUnicodeCategory(chr)
                             where cui != UnicodeCategory.NonSpacingMark
                             select chr;

            var sb = new StringBuilder();
            foreach (var c in normalized)
                sb.Append(c);

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        
    }
}