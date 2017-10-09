#region # using statements #

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#endregion

namespace Scalider.Reflection
{

    /// <summary>
    /// Provides extension methods for the <see cref="Type"/> class.
    /// </summary>
    public static class ReflectionExtensions
    {

        /// <summary>
        /// Determines whether the <paramref name="this"/> is an anonymous
        /// <see cref="Type"/>.
        /// </summary>
        /// <param name="this">The type to test.</param>
        /// <returns>
        /// true if the give <paramref name="this"/> is an anonymous
        /// <see cref="Type"/>; otherwise, false.
        /// </returns>
        public static bool IsAnonymousType(this Type @this)
        {
            Check.NotNull(@this, nameof(@this));

            // HACK: The only way to detect anonymous types right now.
            var typeInfo = @this.GetTypeInfo();
            return
                typeInfo.IsDefined(typeof(CompilerGeneratedAttribute), false) &&
                typeInfo.IsGenericType && @this.Name.Contains("AnonymousType") &&
                (@this.Name.StartsWith("<>") || @this.Name.StartsWith("VB$")) &&
                (typeInfo.Attributes & TypeAttributes.NotPublic) ==
                TypeAttributes.NotPublic;
        }

    }

}