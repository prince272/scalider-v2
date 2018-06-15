using System.Collections.Generic;
using JetBrains.Annotations;

namespace Scalider.Collections
{

    /// <summary>
    /// Provides extension methods for the <see cref="ICollection{T}"/> interface.
    /// </summary>
    public static class CollectionExtensions
    {

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="targetCollection">The <see cref="ICollection{T}"/> to add the elements to.</param>
        /// <param name="collection">The collection whose elements should be added to the
        /// <see cref="ICollection{T}"/>. The collection itself cannot be null, but it can contain elements that are
        /// <c>null</c>, if type <typeparamref name="T"/> is a reference type.</param>
        public static void AddRange<T>([NotNull] this ICollection<T> targetCollection,
            [NotNull] IEnumerable<T> collection)
        {
            Check.NotNull(targetCollection, nameof(targetCollection));
            Check.NotNull(collection, nameof(collection));

            // Determine whether the target collection is an instace of the List class
            if (targetCollection is List<T> list)
            {
                // A list can handle adding ranges by itself
                list.AddRange(collection);
                return;
            }

            // Otherwise, we need to add one by one
            foreach (var item in collection)
                targetCollection.Add(item);
        }

    }

}