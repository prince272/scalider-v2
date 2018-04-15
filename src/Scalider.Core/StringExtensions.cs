using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Scalider
{

    /// <summary>
    /// Provides extension methods for the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// Encodes all the characters in the specified string into a sequence of bytes.
        /// </summary>
        /// <param name="s">The string to encode</param>
        /// <returns>
        /// A byte array containing the results of encoding the specified set of characters.
        /// </returns>
        [SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
        public static byte[] ToByteArray([NotNull] this string s) =>
            string.IsNullOrEmpty(s)
                ? Array.Empty<byte>()
                : Encoding.ASCII.GetBytes(s);

        /// <summary>
        /// Decodes all the bytes in the specified byte array into a string.
        /// </summary>
        /// <param name="bytes">The byte array to decode.</param>
        /// <returns>
        /// A string that contains the results of decoding the specified sequence of bytes.
        /// </returns>
        public static string GetString([NoEnumeration] this IEnumerable<byte> bytes)
        {
            var bytesArray = bytes?.ToArray() ?? Array.Empty<byte>();
            return bytesArray.Length == 0
                ? string.Empty
                : Encoding.ASCII.GetString(bytesArray);
        }

        /// <summary>
        /// Converts the given <paramref name="bytes"/> to its hexadecimal representation.
        /// </summary>
        /// <param name="bytes">The bytes to convert to a hexadecimal string.</param>
        /// <returns>
        /// The hexadecimal representation of the given <paramref name="bytes"/>.
        /// </returns>
        public static string ToHexString(
            [NoEnumeration] this IEnumerable<byte> bytes)
        {
            var bytesArray = bytes?.ToArray() ?? Array.Empty<byte>();
            return bytesArray.Length == 0
                ? string.Empty
                : BitConverter.ToString(bytesArray).Replace("-", "");
        }

        /// <summary>
        /// Encodes all the hexadecimal pair of characters in the specified string into a sequence of bytes.
        /// </summary>
        /// <param name="s">The string to encode</param>
        /// <returns>
        /// A byte array containing the results of encoding the specified set of characters.
        /// </returns>
        public static byte[] FromHexString(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return Array.Empty<byte>();

            return Enumerable.Range(0, s.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(s.Substring(x, 2), 16))
                             .ToArray();
        }

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
        /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
        /// </summary>
        /// <param name="str">The string to truncate.</param>
        /// <param name="maxLength">The maximum length of the truncated string.</param>
        /// <returns>
        /// The truncated string.
        /// </returns>
        [NotNull]
        public static string Truncate(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str) || maxLength <= 0)
                return string.Empty;

            return str.Length <= maxLength ? str : str.Substring(0, maxLength).Trim();
        }

        /// <summary>
        /// <para>
        /// Gets a substring of a string from beginning of the string if it exceeds maximum length. It adds a "..."
        /// postfix to end of the string if it's truncated.
        /// </para>
        /// <para>
        /// Returning string can not be longer than
        /// </para>
        /// <paramref name="maxLength"/>.
        /// </summary>
        /// <param name="str">The string to truncate.</param>
        /// <param name="maxLength">The maximum length of the truncated string.</param>
        /// <returns>
        /// The truncated string.
        /// </returns>
        [NotNull]
        public static string TruncateWithPostfix(this string str, int maxLength) =>
            TruncateWithPostfix(str, maxLength, "...");

        /// <summary>
        /// <para>
        /// Gets a substring of a string from beginning of the string if it exceeds maximum length. It adds given
        /// <paramref name="postfix"/> to end of the string if it's truncated.
        /// </para>
        /// <para>
        /// Returning string can not be longer than
        /// </para>
        /// <paramref name="maxLength"/>.
        /// </summary>
        /// <param name="str">The string to truncate.</param>
        /// <param name="maxLength">The maximum length of the truncated string.</param>
        /// <param name="postfix">The string to append to the truncated string, if the string is truncated.</param>
        /// <returns>
        /// The truncated string.
        /// </returns>
        [NotNull, UsedImplicitly]
        public static string TruncateWithPostfix(this string str, int maxLength,
            [NotNull] string postfix)
        {
            if (string.IsNullOrEmpty(str) || maxLength <= 0)
                return string.Empty;
            
            if (str.Length <= maxLength)
                return str;
            if (maxLength <= postfix.Length)
                return postfix.Substring(0, maxLength);

            return str.Substring(0, maxLength - postfix.Length).Trim() + postfix;
        }

    }

}