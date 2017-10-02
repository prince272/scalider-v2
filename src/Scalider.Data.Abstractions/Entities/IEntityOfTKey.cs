#region # using statements #

using System;

#endregion

namespace Scalider.Data.Entities
{

    /// <summary>
    /// A database entity with a primary key of <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">The type encapsulating the primary key of the
    /// entity.</typeparam>
    public interface IEntity<out TKey> : IEntity
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Gets a value indicating the primary key of the entity in the database.
        /// </summary>
        TKey Id { get; }

    }

}