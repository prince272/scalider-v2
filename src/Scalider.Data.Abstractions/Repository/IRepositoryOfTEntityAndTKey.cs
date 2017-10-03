#region # using statements #

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scalider.Data.Entities;

#endregion

namespace Scalider.Data.Repository
{

    /// <summary>
    /// A repository represents a session with the database that can be used to
    /// retrieve, update and delete entities of type <typeparamref name="TEntity"/>
    /// and with a primary key of <typeparamref name="TKey"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IRepository<TEntity, in TKey> : IRepository<TEntity>
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Returns the entity with the given primary key or <c>null</c> if no
        /// entity with the given primary key is found.
        /// </summary>
        /// <param name="id">The primary key of the entity to retrieve.</param>
        /// <returns>
        /// The entity matching the primary key or <c>null</c> if no entity is
        /// found.
        /// </returns>
        TEntity Find([NotNull] TKey id);

        /// <summary>
        /// Asynchronously returns the entity with the given primary key or
        /// <c>null</c> if no entity with the given primary key is found.
        /// </summary>
        /// <param name="id">The primary key of the entity to retrieve.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<TEntity> FindAsync([NotNull] TKey id);

        /// <summary>
        /// Asynchronously returns the entity with the given primary key or
        /// <c>null</c> if no entity with the given primary key is found.
        /// </summary>
        /// <param name="id">The primary key of the entity to retrieve.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<TEntity> FindAsync([NotNull] TKey id,
            CancellationToken cancellationToken);

    }

}