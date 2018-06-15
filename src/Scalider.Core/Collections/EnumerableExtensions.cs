using System.Collections.Generic;
using System.Linq;

namespace Scalider.Collections
{

    /// <summary>
    /// Provides extension methods for the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class EnumerableExtensions
    {

        /// <summary>
        /// Determines whether the given <see cref="IEnumerable{T}"/> is <c>null</c> or have zero elements.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/>.</param>
        /// <returns>
        /// <c>true</c> when <paramref name="source"/> is not <c>null</c> and have at least one element;
        /// otherwsie, false.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => source == null || !source.Any();

    }
}