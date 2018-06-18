using System;
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
    /// <typeparam name="TKey">The type encapsulating the identifier of the entity.</typeparam>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IRepository<TEntity, in TKey> : IRepository<TEntity>
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Returns the entity with the given identifier or <c>null</c> if no entity is found.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <returns>
        /// The entity matching the identifier or <c>null</c> if no entity is found.
        /// </returns>
        TEntity FindById([NotNull] TKey id);

        /// <summary>
        /// Asynchronously returns the entity with the given identifier or <c>null</c> if no entity is found.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous peration.
        /// </returns>
        Task<TEntity> FindByIdAsync([NotNull] TKey id, CancellationToken cancellationToken = default);

    }

}