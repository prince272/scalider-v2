using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
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
        /// <param name="encoding">The <see cref="Encoding"/>.</param>
        /// <returns>
        /// A byte array containing the results of encoding the specified set of characters.
        /// </returns>
        [SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global")]
        public static byte[] ToByteArray([NotNull] this string s, Encoding encoding = null) =>
            string.IsNullOrEmpty(s)
                ? Array.Empty<byte>()
                : (encoding ?? Encoding.ASCII).GetBytes(s);

        /// <summary>
        /// Decodes all the bytes in the specified byte array into a string.
        /// </summary>
        /// <param name="bytes">The byte array to decode.</param>
        /// <param name="encoding">The <see cref="Encoding"/>.</param>
        /// <returns>
        /// A string that contains the results of decoding the specified sequence of bytes.
        /// </returns>
        public static string GetString([NoEnumeration] this IEnumerable<byte> bytes, Encoding encoding = null)
        {
            var bytesArray = bytes?.ToArray() ?? Array.Empty<byte>();
            return bytesArray.Length == 0
                ? string.Empty
                : (encoding ?? Encoding.ASCII).GetString(bytesArray);
        }

        /// <summary>
        /// Converts the given <paramref name="bytes"/> to its hexadecimal representation.
        /// </summary>
        /// <param name="bytes">The bytes to convert to a hexadecimal string.</param>
        /// <returns>
        /// The hexadecimal representation of the given <paramref name="bytes"/>.
        /// </returns>
        public static string ToHexString([NoEnumeration] this IEnumerable<byte> bytes)
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

    }
}