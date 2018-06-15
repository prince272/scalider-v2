﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scalider.Domain.Entity;

namespace Scalider.Domain.Repository
{

    /// <summary>
    /// A repository represents a session with the data store that can be used to retrieve, update and delete entities
    /// of <typeparamref name="TEntity"/>. 
    /// </summary>
    /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IRepository<TEntity> : IRepository
        where TEntity : class, IEntity
    {

        /*

        /// <summary>
        /// Gets the count of all entities in this repository.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <returns>
        /// The number of entities in this repository.
        /// </returns>
        int Count([NotNull] Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously gets the count of all entities in this repository.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<int> CountAsync([NotNull] Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of all entities in this repository.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <returns>
        /// The number of entities in this repository.
        /// </returns>
        /// <remarks>
        /// Use this method when the return value is expected to be greater
        /// than <see cref="int.MaxValue"/>.
        /// </remarks>
        long LongCount([NotNull] Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously gets the count of all entities in this repository.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        /// <remarks>
        /// Use this method when the return value is expected to be greater
        /// than <see cref="int.MaxValue"/>.
        /// </remarks>
        Task<long> LongCountAsync(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);*/

        /// <summary>
        /// Returns all the entities for this repository.
        /// </summary>
        /// <returns>
        /// A collection containing all the entities for this repository.
        /// </returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Asynchronously returns all the entities for this repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default); /*

        /// <summary>
        /// Returns collection containing all the entities that satisfies a
        /// condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <returns>
        /// The collection containing all the entities that satisfies the
        /// condition.
        /// </returns>
        IEnumerable<TEntity> Find(
            [NotNull] Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously returns collection containing all the entities that
        /// satisfies a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<IEnumerable<TEntity>> FindAsync(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the only entity that satisfies a condition or throws an
        /// exception if there is more than exactly one entity that satisfies
        /// the condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <returns>
        /// The single entity that satisfies the condition.
        /// </returns>
        TEntity Single([NotNull] Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously returns the only entity that satisfies a condition
        /// or throws an exception if there is more than exactly one entity
        /// that satisfies the condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<TEntity> SingleAsync(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the first entity that satisfies a condition or throws an
        /// exception if there is no entity that satisfies the condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <returns>
        /// The first entity that satisfies the condition.
        /// </returns>
        TEntity First([NotNull] Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously returns the first entity that satisfies a condition or
        /// throws an exception if there is no entity that satisfies the condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<TEntity> FirstAsync([NotNull] Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the first entity that satisfies a condition or <c>null</c>
        /// if no such entity is found.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <returns>
        /// The entity that satisfies the condition or <c>null</c> if no such
        /// entity is found.
        /// </returns>
        TEntity FirstOrDefault([NotNull] Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Asynchronously returns the first entity that satisfies a
        /// condition or <c>null</c> if no such entity is found.
        /// </summary>
        /// <param name="predicate">A function to test each element for a
        /// condition.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<TEntity> FirstOrDefaultAsync(
            [NotNull] Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);*/

        /// <summary>
        /// Adds a new entity to the data store.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add([NotNull] TEntity entity);

        /// <summary>
        /// Asynchronously adds a new entity to the data store.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task AddAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing entity on the data store.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update([NotNull] TEntity entity);

        /// <summary>
        /// Asynchronously pdates an existing entity on the data store.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        Task UpdateAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an entity from the data store.
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