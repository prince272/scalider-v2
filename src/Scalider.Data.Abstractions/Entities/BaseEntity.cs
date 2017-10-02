#region # using statements #

using System.Reflection;

#endregion

namespace Scalider.Data.Entities
{

    /// <summary>
    /// Provides a base class for implementations of the <see cref="IEntity"/>
    /// interface.
    /// </summary>
    public abstract class BaseEntity : IEntity
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEntity"/> class.
        /// </summary>
        protected BaseEntity()
        {
        }

        #region # Methods #

        #region == Overrides ==

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            // Must have a IS-A relation of type or must be the same type
            var typeOfThis = GetType().GetTypeInfo();
            var typeOfOther = obj.GetType().GetTypeInfo();

            if (!typeOfThis.IsAssignableFrom(typeOfOther) &&
                !typeOfOther.IsAssignableFrom(typeOfThis))
                return false;

            // Done
            return Equals((BaseEntity)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region == Public ==

        /// <summary>
        /// Indicates whether the values of two specified
        /// <see cref="BaseEntity" /> objects are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> and
        /// <paramref name="right" /> are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(BaseEntity left, BaseEntity right) =>
            Equals(left, null) ? Equals(right, null) : left.Equals(right);

        /// <summary>
        /// Indicates whether the values of two specified
        /// <see cref="BaseEntity" /> objects are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> and
        /// <paramref name="right" /> are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(BaseEntity left, BaseEntity right) =>
            !(left == right);

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
        protected abstract bool Equals(BaseEntity other);

        #endregion

        #endregion

    }

}