using System;
using JetBrains.Annotations;

namespace Scalider
{

    /// <summary>
    /// Provides extension methods for the <see cref="Uri"/> class.
    /// </summary>
    [UsedImplicitly]
    public static class UriExtensions
    {

        /// <summary>
        /// Changes the host of the given <paramref name="uri"/> to the IDN host when needed.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to convert.</param>
        /// <returns>
        /// The converted <see cref="Uri"/>.
        /// </returns>
        [UsedImplicitly]
        public static Uri ToDnsSafeUri([NotNull] this Uri uri)
        {
            Check.NotNull(uri, nameof(uri));
            if (!uri.IsAbsoluteUri)
                return uri;

            // Determine if should change the host for the given uri
            var host = uri.Host;
            var idnHost = uri.IdnHost;
            if (string.IsNullOrEmpty(idnHost) || string.Equals(host, idnHost))
            {
                // No need to change the host
                return uri;
            }

            // Change the host and done
            var builder = new UriBuilder(uri) {Host = idnHost};
            return builder.Uri;
        }

    }
}