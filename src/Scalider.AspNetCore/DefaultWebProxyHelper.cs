using System;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Scalider.AspNetCore
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IWebProxyHelper"/> interface.
    /// </summary>
    [UsedImplicitly]
    public class DefaultWebProxyHelper : IWebProxyHelper
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

        /// <inheritdoc />
        public virtual bool IsHttps(HttpContext httpContext)
        {
            Check.NotNull(httpContext, nameof(httpContext));

            // Retrieve the current request and determine if its marked as HTTPS
            var request = httpContext.Request;
            if (request == null)
                return false;

            if (request.IsHttps)
            {
                // The request seems to be an HTTPS request, so we don't need to do anything
                return true;
            }

            // Try to retrieve the forwarded header and determine if it corresponds to HTTPS
            foreach (var headerName in PossibleForwardedProtocolHeaders)
            {
                if (!TryGetFirstNotEmptyHeaderValue(request, headerName, out var headerValue))
                    continue;

                if (string.Equals(headerValue, "https", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(headerValue, "on", StringComparison.OrdinalIgnoreCase))
                {
                    // The forwarded header seems indicate that the request is HTTPS
                    return true;
                }
            }

            // The request doesn't seem to be HTTPS
            return false;
        }

        /// <inheritdoc />
        public virtual bool TryGetHost(HttpContext httpContext, out string result)
        {
            result = null;
            if (httpContext?.Request == null)
                return false;

            var request = httpContext.Request;

            // Try to retrieve the forwarded header
            foreach (var headerName in PossibleHostHeaders)
            {
                if (!TryGetFirstNotEmptyHeaderValue(request, headerName, out var headerValue))
                    continue;

                result = headerValue;
                return true;
            }

            // Could not retrieve the host from the forwarded headers
            var requestHost = request.Host;
            if (!requestHost.HasValue || string.IsNullOrWhiteSpace(requestHost.Value))
                return false;

            result = requestHost.Value;
            return true;
        }

        /// <inheritdoc />
        public virtual bool TryGetRemoteIpAddress(HttpContext httpContext, out IPAddress result)
        {
            result = null;
            if (httpContext?.Request == null)
                return false;

            // Walk thru headers until we find a valid IP address
            var request = httpContext.Request;
            foreach (var headerName in PossibleRemoteIpAddressHeaders)
            {
                if (!TryGetFirstNotEmptyHeaderValue(request, headerName, out var headerValue))
                    continue;

                // Retrieve the real header value
                string possibleIpAddress;
                if (string.Equals("X-Forwarded-For", headerName, StringComparison.OrdinalIgnoreCase))
                {
                    // We could get a comma separated list of IP addresses
                    // See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-For
                    var possibleIpAddresses = headerValue.Split(
                        new[] {','},
                        StringSplitOptions.RemoveEmptyEntries
                    );

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

                result = ipAddress;
                return true;
            }

            // Could not retrieve the remote IP address from the forwarded headers
            var connection = httpContext.Connection;
            if (connection?.RemoteIpAddress == null)
                return false;

            result = connection.RemoteIpAddress;
            return true;
        }

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

    }

}