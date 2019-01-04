using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Scalider.Collections;

namespace Scalider.Globalization
{

    /// <summary>
    /// Provides some helper methods for retrieving and validating <see cref="CultureInfo"/>.
    /// </summary>
    public static class CultureInfoHelper
    {

        private static HashSet<string> _allKnownCultureInfoNames;
        private static readonly ConcurrentDictionary<string, CultureInfo> CultureInfoCache =
            new ConcurrentDictionary<string, CultureInfo>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Determines whether the given <paramref name="cultureName"/> is a valid culture name.
        /// </summary>
        /// <param name="cultureName">The name of the culture to validate.</param>
        /// <returns>
        /// <c>true</c> if the given <paramref name="cultureName"/> is a valid culture name;
        /// otherwise, <c>false</c>.
        /// </returns>
        [UsedImplicitly]
        public static bool IsKnownCultureName([NotNull] string cultureName)
        {
            if (string.IsNullOrWhiteSpace(cultureName))
            {
                // The provided culture name seems to be null or blank
                return false;
            }

            // Determine whether the list of known culture names has been built
            if (_allKnownCultureInfoNames != null)
            {
                // The list of known culture names has already been built, determine whether the provided
                // culture name exists
                return _allKnownCultureInfoNames.Contains(cultureName);
            }

            // The list of known culture names hasn't been built, lets build it. We are using a case insensitive
            // comparator, that way "en" and "EN" mean the same thing
            var knownCultureNames = CultureInfo
                .GetCultures(CultureTypes.AllCultures /* & ~CultureTypes.NeutralCultures*/)
                .Where(t => !string.IsNullOrWhiteSpace(t.Name))
                .Select(t => t.Name);

            _allKnownCultureInfoNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _allKnownCultureInfoNames.AddRange(knownCultureNames);

            // Now, determine whether the given culture name exists in the list of known culture names
            return _allKnownCultureInfoNames.Contains(cultureName);
        }

        /// <summary>
        /// Retrieves a cached, read-only instance of the culture using the provided culture name. If the
        /// culture is not supported, or the culture name is invalid, <c>NULL</c> is returned instead.
        /// </summary>
        /// <param name="cultureName">The name of the culture to retrieve.</param>
        /// <returns>
        /// A read-only <see cref="CultureInfo"/> representing the given culture name or <c>NULL</c> if the
        /// culture is not supported or the culture name is invalid.
        /// </returns>
        [CanBeNull]
        [UsedImplicitly]
        public static CultureInfo SafeGetCultureInfo([NotNull] string cultureName)
        {
            if (!IsKnownCultureName(cultureName))
            {
                // The provided culture is not valid
                return null;
            }

            // Try to retrieve the CultureInfo for the provided culture name
            try
            {
                return CultureInfoCache.GetOrAdd(cultureName, CultureInfo.GetCultureInfo);
            }
            catch (CultureNotFoundException)
            {
                // ignore
            }

            // The provided culture is not supported
            return null;
        }

    }

}