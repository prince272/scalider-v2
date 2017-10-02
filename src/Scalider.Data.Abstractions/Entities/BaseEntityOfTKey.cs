#region # using statements #

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Scalider.Data.Entities
{

    /// <summary>
    /// Provides a base class for implementations of the <see cref="IEntity{TKey}"/>
    /// generic interface.
    /// </summary>
    /// <typeparam name="TKey">The type encapsulating the primary key of the
    /// entity.</typeparam>
    public abstract class BaseEntity<TKey> : BaseEntity, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity{TKey}"/> class.
        /// </summary>
        protected BaseEntity()
        {
        }

        #region # Methods #

        #region == Overrides ==

        /// <inheritdoc />
        public override string ToString() => $"{GetType().Name} {Id}";

        /// <inheritdoc />
        protected sealed override bool Equals(BaseEntity other) =>
            Equals((BaseEntity<TKey>)other);

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            Debug.Assert(Id != null, nameof(Id) + " != null");
            return EqualityComparer<TKey>.Default.GetHashCode(Id);
        }

        #endregion

        #region == Protected ==

        /// <summary>
        /// Determines whether the specified object is equal to the current
        /// object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// true if the specified object is equal to the current object;
        /// otherwise, false.
        /// </returns>
        protected virtual bool Equals(BaseEntity<TKey> other) =>
            EqualityComparer<TKey>.Default.Equals(Id, other.Id);

        #endregion

        #endregion

        #region # IEntity<TKey> #

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public virtual TKey Id { get; protected set; }

        #endregion

    }

}