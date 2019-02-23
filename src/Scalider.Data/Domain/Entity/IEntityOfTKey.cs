using System;
using JetBrains.Annotations;

namespace Scalider.Domain.Entity
{

    /// <summary>
    /// Represents an entity that can be persisted using the database and have an identifier of
    /// <typeparamref name="TKey" />.
    /// </summary>
    /// <typeparam name="TKey">The type encapsulating the identity of the entity.</typeparam>
    public interface IEntity<out TKey> : IEntity
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Gets a value indicating the primary key of the entity in the database.
        /// </summary>
        [UsedImplicitly]
        TKey Id { get; }

    }

}