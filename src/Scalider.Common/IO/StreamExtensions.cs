#region # using statements #

using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Scalider.IO
{

    /// <summary>
    /// Provides extension methods for the <see cref="Stream"/> class.
    /// </summary>
    public static class StreamExtensions
    {

        /// <summary>
        /// Retrieves all the bytes from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to retrieve the bytes
        /// from.</param>
        /// <returns>
        /// The bytes from the given <paramref name="stream"/>.
        /// </returns>
        [NotNull]
        public static byte[] ToArray([NotNull] this Stream stream)
        {
            Check.NotNull(stream, nameof(stream));

            // Determine if the given stream is a MemoryStream, if so, we can
            // just use its own method instead
            if (stream is MemoryStream memoryStream)
                return memoryStream.ToArray();

            // Try to copy stream to a MemoryStream
            try
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            catch
            {
                // Failed to copy stream to a MemoryStream
            }

            // Failed to retrieve stream bytes
            return Array.Empty<byte>();
        }

        /// <summary>
        /// Asynchronously retrieves all the bytes from the given
        /// <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to retrieve the bytes
        /// from.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        public static async Task<byte[]> ToArrayAsync([NotNull] this Stream stream)
        {
            Check.NotNull(stream, nameof(stream));

            // Determine if the given stream is a MemoryStream, if so, we can
            // just use its own method instead
            if (stream is MemoryStream memoryStream)
                return memoryStream.ToArray();

            // Try to copy stream to a MemoryStream
            try
            {
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
            catch
            {
                // Failed to copy stream to a MemoryStream
            }

            // Failed to retrieve stream bytes
            return Array.Empty<byte>();
        }

    }
}