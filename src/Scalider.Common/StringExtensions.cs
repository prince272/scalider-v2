#if NETSTANDARD1_7
using System;
#endif
using System.Globalization;
using System.Linq;
using System.Text;

namespace Scalider
{
    
    /// <summary>
    /// Provides extension methods for the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {

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