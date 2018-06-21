using System;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Scalider.AspNetCore
{
    
    /// <summary>
    /// Provides extension methods for the <see cref="HttpContext"/> class.
    /// </summary>
    public static class HttpContextExtensions
    {

        private static readonly string[] PossibleForwardedProtocolHeaders =
        {
            "X-Forwarded-Proto", // This is usually used by proxies,
            // These are non-standard headers
            "X-Forwarded-Protocol",
            "X-Url-Scheme",
            "Front-End-Https", // Microsoft
            "X-Forwarded-Ssl"
        };

        private static readonly string[] PossibleHostHeaders =
        {
            "X-Forwarded-Host" // This is usually used by proxies
        };

        private static readonly string[] PossibleRemoteIpAddressHeaders =
        {
            "True-Client-IP", // This is a feature of Cloudflare Enterprise and for SSR requests
            "CF-Connecting-IP", // This is a feature of Cloudflare
            "X-Forwarded-For", // This is usually used by proxies
            "X-ProxyUser-Ip", // This is a non-standard form
            "X-Real-IP" // This is a non-standard form
        };
        
        #region IsHttps

        /// <summary>
        /// Determines whether the request protocol used by the client is HTTPS. This will also take into account the
        /// forwarded headers when the request itself isn't HTTPS.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
        /// <returns>
        /// <c>true</c> if the request protocol is HTTPS; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsHttps([NotNull] this HttpContext httpContext)
        {
            Check.NotNull(httpContext, nameof(httpContext));
            if (httpContext.Request.IsHttps)
            {
                // The request is HTTPS, no need to check forwarded header
                return true;
            }
            
            // Try to retrieve the forwarded header and determine if it corresponds to HTTPS
            foreach (var headerName in PossibleForwardedProtocolHeaders)
            {
                if (!TryGetFirstNotEmptyHeaderValue(httpContext.Request, headerName, out var headerValue))
                {
                    // The attempted header doesn't exists
                    continue;
                }

                // Determine if the header value is HTTPS
                if (string.Equals("https", headerValue, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals("on", headerName, StringComparison.OrdinalIgnoreCase))
                {
                    // The header value is HTTPS
                    return true;
                }
            }

            // The request doesn't seem to be HTTPS
            return false;
        }
        
        #endregion
        
        #region TryGetTrueHost

        /// <summary>
        /// Retrieves the true requested host. This will also take into account the forwarded headers.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
        /// <param name="resultOutput">An out variable where the result will be set.</param>
        /// <returns>
        /// <c>true</c> if could retrieve the host; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryGetTrueHost(this HttpContext httpContext, out string resultOutput)
        {
            resultOutput = null;
            if (httpContext == null)
            {
                return false;
            }
            
            // Try to retrieve the forwarded header
            foreach (var headerName in PossibleHostHeaders)
            {
                if (!TryGetFirstNotEmptyHeaderValue(httpContext.Request, headerName, out var headerValue))
                    continue;
                
                resultOutput = headerValue;
                return true;
            }
            
            // Could not retrieve the host from the forwarded headers
            if (!httpContext.Request.Host.HasValue)
                return false;

            resultOutput = httpContext.Request.Host.Value;
            return true;
        }
        
        #endregion
        
        #region TryGetTrueClientIpAddress

        /// <summary>
        /// Tries to to retrieve the true client IP address.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
        /// <param name="resultOutput">An out variable where the result will be set.</param>
        /// <returns>
        /// <c>true</c> if could retrieve a valid IP address for the client; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryGetTrueClientIpAddress(this HttpContext httpContext, out IPAddress resultOutput)
        {
            resultOutput = null;
            if (httpContext == null)
            {
                // No HttpContext provided
                return false;
            }
            
            // Walk thru headers until we find a valid IP address
            foreach (var headerName in PossibleRemoteIpAddressHeaders)
            {
                if (!TryGetFirstNotEmptyHeaderValue(httpContext.Request, headerName, out var headerValue))
                    continue;

                // Retrieve the real header value
                string possibleIpAddress;
                if (string.Equals("X-Forwarded-For", headerName, StringComparison.OrdinalIgnoreCase))
                {
                    // We could get a comma separated list of IP addresses
                    // See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-For
                    var possibleIpAddresses = headerValue.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    if (!possibleIpAddresses.Any())
                        continue;

                    possibleIpAddress = possibleIpAddresses.First();
                }
                else
                    possibleIpAddress = headerValue;

                // Determine if the possible value is a valid IP address
                if (string.IsNullOrWhiteSpace(possibleIpAddress) ||
                    !IPAddress.TryParse(possibleIpAddress.Trim(), out var ipAddress))
                    continue;
                
                resultOutput = ipAddress;
                return true;
            }
            
            // Could not retrieve the remote IP address from the forwarded headers
            if (httpContext.Connection?.RemoteIpAddress == null)
                return false;

            resultOutput = httpContext.Connection.RemoteIpAddress;
            return true;
        }
        
        #endregion
        
        #region TryGetFirstNotEmptyHeaderValue

        private static bool TryGetFirstNotEmptyHeaderValue(HttpRequest httpRequest, string headerName,
            out string resultOutput)
        {
            resultOutput = null;
            if (httpRequest == null || string.IsNullOrWhiteSpace(headerName))
            {
                // The request object wasn't provided or the header name is empty
                return false;
            }
            
            // Determine if there is at leas a single value for the header
            var headerValues = httpRequest.Headers[headerName];
            if (!headerName.Any())
            {
                // There is no header with the given name
                return false;
            }

            var firstHeaderValue = headerValues.FirstOrDefault(t => !string.IsNullOrEmpty(t));
            if (string.IsNullOrEmpty(firstHeaderValue))
            {
                // The header doesn't have any value or the values of all the headers are empty
                return false;
            }
            
            // Done
            resultOutput = firstHeaderValue;
            return true;
        }
        
        #endregion
        
    }
    
}