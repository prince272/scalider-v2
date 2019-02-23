using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scalider.Domain.Entity;

namespace Scalider.Domain.Repository
{

    /// <summary>
    /// Provides extension methods for the <see cref="IRepository"/> interface.
    /// </summary>
    public static class RepositoryExtensions
    {

        /// <summary>
        /// Removes the entity with the given identifier.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="id">The identifier of the entity to remove.</param>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <typeparam name="TKey">The type encapsulating the identifier of the entity.</typeparam>
        [UsedImplicitly]
        public static void RemoveById<TEntity, TKey>([NotNull] this IRepository<TEntity, TKey> repository, TKey id)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            Check.NotNull(repository, nameof(repository));
            if (EqualityComparer<TKey>.Default.Equals(id, default))
            {
                // The given identifier seems to be the default value for the type
                return;
            }

            // Try to retrieve the entity with the given unique identifier
            var entity = repository.FindById(id);
            if (entity != null)
            {
                // The entity was found, remove it
                repository.Remove(entity);
            }
        }

        /// <summary>
        /// Asynchronously removes the entity with the given identifier.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="id">The identifier of the entity to remove.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <typeparam name="TKey">The type encapsulating the identifier of the entity.</typeparam>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous peration.
        /// </returns>
        [UsedImplicitly]
        public static async Task RemoveByIdAsync<TEntity, TKey>([NotNull] this IRepository<TEntity, TKey> repository,
            TKey id, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            Check.NotNull(repository, nameof(repository));
            if (EqualityComparer<TKey>.Default.Equals(id, default))
            {
                // The given identifier seems to be the default value for the type
                return;
            }

            // Try to retrieve the entity with the given unique identifier
            var entity = await repository.FindByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                // The entity was found, remove it
                await repository.RemoveAsync(entity, cancellationToken);
            }
        }

    }

}