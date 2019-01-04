using System;
using System.Text;
using JetBrains.Annotations;
using Scalider.Globalization;

namespace Scalider
{

    /// <summary>
    /// Provides some utility methods for dealing with Uris and Uris related operations.
    /// </summary>
    [UsedImplicitly]
    public static class UriUtility
    {

        /// <summary>
        /// Transforms the string into a URL-friendly slug.
        /// </summary>
        /// <param name="str">The original string.</param>
        /// <returns>
        /// A string containing a url-friendly slug.
        /// </returns>
        [UsedImplicitly]
        [NotNull]
        public static string ToFriendlyUri(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            // Remove any special characters
            var sb = new StringBuilder();
            foreach (var chr in str.Trim().ToLower().RemoveDiacritics())
            {
                if (chr <= 'z' && chr >= 'a' || chr <= '9' && chr >= '0')
                    sb.Append(chr);
                else
                    sb.Append('-');
            }

            // Remove double dashes
            var output = sb.ToString().Normalize(NormalizationForm.FormC).Trim('-');
            while (output.IndexOf("--", StringComparison.Ordinal) >= 0)
                output = output.Replace("--", "-");

            // Return generated result
            return output;
        }

    }

}