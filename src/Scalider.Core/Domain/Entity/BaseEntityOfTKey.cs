﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Scalider.Domain.Entity
{

    /// <summary>
    /// Provides a base class for implementations of the <see cref="IEntity{TKey}"/> generic interface.
    /// </summary>
    /// <typeparam name="TKey">The type encapsulating the identity of the entity.</typeparam>
    public abstract class BaseEntity<TKey> : BaseEntity, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity{TKey}"/> class.
        /// </summary>
        protected BaseEntity()
        {
        }

        /// <inheritdoc />
        public override string ToString() => $"{GetType().Name} {Id}";

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override bool Equals(BaseEntity other) =>
            other is BaseEntity<TKey> entity && Equals(entity);

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            Debug.Assert(Id != null, nameof(Id) + " != null");
            return EqualityComparer<TKey>.Default.GetHashCode(Id);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        protected virtual bool Equals(BaseEntity<TKey> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            
            return ReferenceEquals(this, other) ||
                   EqualityComparer<TKey>.Default.Equals(Id, other.Id);
        }

        #region IEntity<TKey> Members

        /// <inheritdoc />
        public virtual TKey Id { get; [UsedImplicitly] set; }

        #endregion

    }

}