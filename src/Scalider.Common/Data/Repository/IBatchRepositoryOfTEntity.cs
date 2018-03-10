using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scalider.Data.Entity;

namespace Scalider.Data.Repository
{
 
    /// <summary>
    /// Provides aditional methods to the repository pattern for supporting batched operations.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBatchRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {

        /// <summary>
        /// Adds a collection of new entities to the data store.
        /// </summary>
        /// <param name="entities">The collection of entities to add to the data store.</param>
        void AddRange([NotNull] IEnumerable<TEntity> entities);

        /// <summary>
        /// Asynchronously adds a collection of new entities to the data store.
        /// </summary>
        /// <param name="entities">The collection of entities to add to the data store.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for
        /// the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task AddRangeAsync([NotNull] IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a collection of entities in the data store.
        /// </summary>
        /// <param name="entities">The collection of entities to update in the data store.</param>
        void UpdateRange([NotNull] IEnumerable<TEntity> entities);

        /// <summary>
        /// Asynchronously updates a collection of entities in the data store.
        /// </summary>
        /// <param name="entities">The collection of entities to update in the data store.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task UpdateRangeAsync([NotNull] IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a collection of entities from the data store.
        /// </summary>
        /// <param name="entities">The collection of entities to remove from the data store.</param>
        void RemoveRange([NotNull] IEnumerable<TEntity> entities);

        /// <summary>
        /// Asynchronously removes a collection of entities from the data store.
        /// </summary>
        /// <param name="entities">The collection of entities to remove from the data store.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task RemoveRangeAsync([NotNull] IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    }
    
}