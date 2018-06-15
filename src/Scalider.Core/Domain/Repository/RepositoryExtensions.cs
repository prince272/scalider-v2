using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scalider.Domain.Entity;

namespace Scalider.Domain.Repository
{

    /// <summary>
    /// Provides extension methods for the <see cref="IRepository"/> interfaces.
    /// </summary>
    public static class RepositoryExtensions
    {

        /// <summary>
        /// Adds a collection of new entities to the data store.
        /// </summary>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The collection of entities to add to the data store.</param>
        public static void AddRange<TEntity>([NotNull] this IRepository<TEntity> repository,
            [NotNull] params TEntity[] entities)
            where TEntity : class, IEntity
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(entities, nameof(entities));

            GetBatchRepositoryOrThrow(repository).AddRange(entities.AsEnumerable());
        }

        /// <summary>
        /// Asynchronously adds a collection of new entities to the data store.
        /// </summary>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The collection of entities to add to the data store.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        public static Task AddRangeAsync<TEntity>([NotNull] this IRepository<TEntity> repository,
            [NotNull] params TEntity[] entities)
            where TEntity : class, IEntity
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(entities, nameof(entities));

            return GetBatchRepositoryOrThrow(repository).AddRangeAsync(entities.AsEnumerable());
        }

        /// <summary>
        /// Asynchronously adds a collection of new entities to the data store.
        /// </summary>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The collection of entities to add to the data store.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        public static Task AddRangeAsync<TEntity>([NotNull] this IRepository<TEntity> repository,
            [NotNull] TEntity[] entities, CancellationToken cancellationToken)
            where TEntity : class, IEntity
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(entities, nameof(entities));

            return GetBatchRepositoryOrThrow(repository).AddRangeAsync(entities.AsEnumerable(), cancellationToken);
        }

        /// <summary>
        /// Updates a collection of entities in the data store.
        /// </summary>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The collection of entities to add to the data store.</param>
        public static void UpdateRange<TEntity>([NotNull] this IRepository<TEntity> repository,
            [NotNull] params TEntity[] entities)
            where TEntity : class, IEntity
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(entities, nameof(entities));

            GetBatchRepositoryOrThrow(repository).UpdateRange(entities);
        }

        /// <summary>
        /// Asynchronously updates a collection of entities in the data store.
        /// </summary>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The collection of entities to add to the data store.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        public static Task UpdateRangeAsync<TEntity>([NotNull] this IRepository<TEntity> repository,
            [NotNull] params TEntity[] entities)
            where TEntity : class, IEntity
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(entities, nameof(entities));

            return GetBatchRepositoryOrThrow(repository).UpdateRangeAsync(entities);
        }

        /// <summary>
        /// Asynchronously updates a collection of entities in the data store.
        /// </summary>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The collection of entities to add to the data store.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        public static Task UpdateRangeAsync<TEntity>([NotNull] this IRepository<TEntity> repository,
            [NotNull] TEntity[] entities, CancellationToken cancellationToken)
            where TEntity : class, IEntity
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(entities, nameof(entities));

            return GetBatchRepositoryOrThrow(repository).UpdateRangeAsync(entities, cancellationToken);
        }

        /// <summary>
        /// Removes a collection of entities from the data store.
        /// </summary>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The collection of entities to remove from the data store.</param>
        public static void RemoveRange<TEntity>([NotNull] this IRepository<TEntity> repository,
            [NotNull] params TEntity[] entities)
            where TEntity : class, IEntity
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(entities, nameof(entities));

            GetBatchRepositoryOrThrow(repository).RemoveRange(entities);
        }

        /// <summary>
        /// Asynchronously removes a collection of entities from the data store.
        /// </summary>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The collection of entities to remove from the data store.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        public static Task RemoveRangeAsync<TEntity>([NotNull] this IRepository<TEntity> repository,
            [NotNull] params TEntity[] entities)
            where TEntity : class, IEntity
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(entities, nameof(entities));

            return GetBatchRepositoryOrThrow(repository).UpdateRangeAsync(entities);
        }

        /// <summary>
        /// Asynchronously removes a collection of entities from the data store.
        /// </summary>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="entities">The collection of entities to remove from the data store.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        public static Task RemoveRangeAsync<TEntity>([NotNull] this IRepository<TEntity> repository,
            [NotNull] TEntity[] entities, CancellationToken cancellationToken)
            where TEntity : class, IEntity
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(entities, nameof(entities));

            return GetBatchRepositoryOrThrow(repository).UpdateRangeAsync(entities, cancellationToken);
        }

        private static IBatchRepository<TEntity> GetBatchRepositoryOrThrow<TEntity>(IRepository<TEntity> repository)
            where TEntity : class, IEntity
        {
            if (repository is IBatchRepository<TEntity> batchRepository)
                return batchRepository;

            // The type of the repository doesn't support batch
            var typeName = ReflectionUtils.GetTypeReadableName(typeof(IBatchRepository<TEntity>));
            throw new ArgumentException($"The repository must implement the {typeName} interface", nameof(repository));
        }

    }

}