using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Scalider.Domain.Entity
{

    /// <summary>
    /// Provides a base class for implementations of the <see cref="IEntity{TKey}"/> generic interface.
    /// </summary>
    /// <typeparam name="TKey">The type encapsulating the identity of the entity.</typeparam>
    public abstract class BaseEntity<TKey> : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <inheritdoc />
        public virtual TKey Id { get; [UsedImplicitly] set; }

        /// <inheritdoc />
        public override string ToString() => $"{GetType().Name} {Id}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && Equals((BaseEntity<TKey>)obj);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode() => EqualityComparer<TKey>.Default.GetHashCode(Id);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        private bool Equals(BaseEntity<TKey> other) =>
            other != null &&
            EqualityComparer<TKey>.Default.Equals(Id, other.Id);

        /// <summary>
        /// Indicates whether the values of two specified <see cref="BaseEntity{TKey}" /> have the same value.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="left" /> is the same as the value of <paramref name="right" />;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(BaseEntity<TKey> left, BaseEntity<TKey> right) =>
            left?.Equals(right) ?? Equals(right, null);

        /// <summary>
        /// Indicates whether the values of two specified <see cref="BaseEntity{TKey}" /> have different values.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="left" /> is different from the value of
        /// <paramref name="right" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(BaseEntity<TKey> left, BaseEntity<TKey> right) => !(left == right);

    }

}