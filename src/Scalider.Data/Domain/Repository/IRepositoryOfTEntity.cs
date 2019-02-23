using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scalider.Domain.Entity;

namespace Scalider.Domain.Repository
{

    /// <summary>
    /// Represents a repository that provides CRUD operations for the speicified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IRepository<TEntity> : IRepository
        where TEntity : class, IEntity
    {

        /// <summary>
        /// Returns all the entities for this repository.
        /// </summary>
        /// <returns>
        /// A collection containing all the entities for this repository.
        /// </returns>
        [UsedImplicitly]
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Asynchronously returns all the entities for this repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        [UsedImplicitly]
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        [UsedImplicitly]
        void Add([NotNull] TEntity entity);

        /// <summary>
        /// Asynchronously adds a new entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        [UsedImplicitly]
        Task AddAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        [UsedImplicitly]
        void Update([NotNull] TEntity entity);

        /// <summary>
        /// Asynchronously updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        Task UpdateAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an existing entity.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove([NotNull] TEntity entity);

        /// <summary>
        /// Asynchronously removes an existing entity.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        Task RemoveAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);

    }

}