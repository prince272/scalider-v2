using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Scalider.Reflection
{

    /// <summary>
    /// Provides extension methods for the <see cref="Type"/> class.
    /// </summary>
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
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

        /// <summary>
        /// Determines whether the given <paramref name="type"/> implements or
        /// inherits the <typeparamref name="T"/> class/interface.
        /// </summary>
        /// <param name="type"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// true when <paramref name="type"/> implements or inherits the
        /// <typeparamref name="T"/> class/interface; otherwise, false.
        /// </returns>
        public static bool ImplementsOrInherits<T>([NotNull] this Type type) =>
            ImplementsOrInherits(type, typeof(T));

        /// <summary>
        /// Determines whether the given <paramref name="type"/> implements or
        /// inherits the <paramref name="otherType"/> class/interface.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="otherType"></param>
        /// <returns>
        /// true when <paramref name="type"/> implements or inherits the
        /// <paramref name="otherType"/> class/interface; otherwise, false.
        /// </returns>
        [UsedImplicitly]
        public static bool ImplementsOrInherits([NotNull] this Type type,
            [NotNull] Type otherType)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(otherType, nameof(otherType));

            var clrType = type.GetTypeInfo();
            var otherClrType = otherType.GetTypeInfo();

            return otherClrType.IsInterface
                ? TypeImplementsInterface(clrType, otherClrType)
                : TypeInheritClass(clrType, otherClrType);
        }

        private static bool HasSameGenericTypeDefinition(TypeInfo type,
            TypeInfo otherType)
        {
            if (!type.IsGenericType)
                return false;
            
            // Determine if the base type definition is the same
            var clrType = otherType.GetGenericTypeDefinition().GetTypeInfo();
            var actualType = type.GetGenericTypeDefinition().GetTypeInfo();

            if (!Equals(actualType, clrType))
                return false;
            
            // Determine if the generic arguments are the same
            if (otherType.IsGenericTypeDefinition)
                return true;

            var myArguments = type.GetGenericArguments();
            var otherArguments = otherType.GetGenericArguments();

            return myArguments.SequenceEqual(otherArguments);
        }

        private static bool TypeImplementsInterface(TypeInfo type,
            TypeInfo otherType)
        {
            if (!otherType.IsInterface)
            {
                throw new ArgumentException(
                    $"The parameter {nameof(otherType)} must be an interface",
                    nameof(otherType)
                );
            }

            var isGeneric = otherType.IsGenericType;
            return type.GetInterfaces().Select(t => t.GetTypeInfo())
                       .Any(t => Equals(t, otherType) || isGeneric &&
                                 HasSameGenericTypeDefinition(t, otherType));
        }

        private static bool TypeInheritClass(TypeInfo type,
            TypeInfo otherType)
        {
            if (!otherType.IsClass)
            {
                throw new ArgumentException(
                    $"The parameter {nameof(otherType)} must be a class",
                    nameof(otherType)
                );
            }
            
            // Determine if the type inherits the required type
            var isGeneric = otherType.IsGenericType;
            var baseType = type.BaseType;
            
            while (baseType != null && baseType != typeof(object))
            {
                var baseClrType = baseType.GetTypeInfo();
                if (Equals(baseClrType, otherType) || isGeneric &&
                    HasSameGenericTypeDefinition(baseClrType, otherType))
                    return true;
                
                baseType = baseClrType.BaseType;
            }

            // The type doesn't meet the criteria
            return false;
        }

    }

}