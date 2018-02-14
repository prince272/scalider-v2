using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scalider.Data.Entity;

namespace Scalider.Data.Repository
{
    
    /// <summary>
    /// Provides aditional methods to the repository pattern that are only
    /// available thanks to the Entity Framework Core.
    /// </summary>
    /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
    public interface IEfRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {

        /// <summary>
        /// Adds a collection of new entities to the data store.
        /// </summary>
        /// <param name="entities">The collection of entities to add to the
        /// data store.</param>
        void AddRange([NotNull] IEnumerable<TEntity> entities);

        /// <summary>
        /// Asynchronously adds a collection of new entities to the data store.
        /// </summary>
        /// <param name="entities">The collection of entities to add to the
        /// data store.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task AddRangeAsync([NotNull] IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default);

    }
    
}