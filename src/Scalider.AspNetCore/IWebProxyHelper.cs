using System.Net;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Scalider.AspNetCore
{

    /// <summary>
    /// Provides helper methods for resolving common properties for the <see cref="HttpContext"/>
    /// when the host has been proxied.
    /// </summary>
    [UsedImplicitly]
    public interface IWebProxyHelper
    {

        /// <summary>
        /// Determines whether the request protocol used by the client is HTTPS. This will also take into account the
        /// forwarded headers when the request itself isn't HTTPS.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
        /// <returns>
        /// <c>true</c> if the request protocol is HTTPS; otherwise, <c>false</c>.
        /// </returns>
        [UsedImplicitly]
        bool IsHttps([NotNull] HttpContext httpContext);

        /// <summary>
        /// Retrieves the true requested host. This will also take into account the forwarded headers.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
        /// <param name="result">An out variable where the result will be set.</param>
        /// <returns>
        /// <c>true</c> if could retrieve the host; otherwise, <c>false</c>.
        /// </returns>
        [UsedImplicitly]
        bool TryGetHost(HttpContext httpContext, out string result);

        /// <summary>
        /// Tries to to retrieve the true client IP address.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
        /// <param name="result">An out variable where the result will be set.</param>
        /// <returns>
        /// <c>true</c> if could retrieve a valid IP address for the client; otherwise, <c>false</c>.
        /// </returns>
        [UsedImplicitly]
        bool TryGetRemoteIpAddress(HttpContext httpContext, out IPAddress result);

    }

}