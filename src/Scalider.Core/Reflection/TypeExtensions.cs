using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Scalider.Reflection
{

    /// <summary>
    /// Provides extension methods for the <see cref="Type"/> class.
    /// </summary>
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public static class TypeExtensions
    {

        /// <summary>
        /// Determines whether the <paramref name="this"/> is an anonymous <see cref="Type"/>.
        /// </summary>
        /// <param name="this">The type to test.</param>
        /// <returns>
        /// <c>true</c> if the give <paramref name="this"/> is an anonymous <see cref="Type"/>; otherwise, <c>false</c>.
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
        /// Determines whether the given <paramref name="type"/> implements or inherits the <typeparamref name="T"/>
        /// class/interface.
        /// </summary>
        /// <param name="type"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// true when <paramref name="type"/> implements or inherits the
        /// <typeparamref name="T"/> class/interface; otherwise, false.
        /// </returns>
        public static bool ImplementsOrInherits<T>([NotNull] this Type type) => ImplementsOrInherits(type, typeof(T));

        /// <summary>
        /// Determines whether the given <paramref name="type"/> implements or inherits the <paramref name="otherType"/>
        /// class/interface.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="otherType"></param>
        /// <returns>
        /// true when <paramref name="type"/> implements or inherits the <paramref name="otherType"/> class/interface;
        /// otherwise, false.
        /// </returns>
        [UsedImplicitly]
        public static bool ImplementsOrInherits([NotNull] this Type type, [NotNull] Type otherType)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(otherType, nameof(otherType));

            var clrType = type.GetTypeInfo();
            var otherClrType = otherType.GetTypeInfo();

            return otherClrType.IsInterface
                ? TypeImplementsInterface(clrType, otherClrType)
                : TypeInheritClass(clrType, otherClrType);
        }

        /// <summary>
        /// Gets the readable name for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        /// A string representing the readable name of the <paramref name="type"/>.
        /// </returns>
        public static string GetReadableName([NotNull] this Type type)
        {
            Check.NotNull(type, nameof(type));
            
            var typeInfo = type.GetTypeInfo();
            var sb = new StringBuilder();
            var name = type.Name;
            
            // Determine if the type is generic
            if (typeInfo.IsGenericType)
            {
                // The type is generic, do some adjustments
                var tildePos = name.IndexOf('`');
                if (tildePos > -1)
                    name = name.Substring(0, tildePos);

                // Retrieve the list of generic arguments, either the parameter or the type name
                var genericArgs = new List<string>();
                genericArgs.AddRange(typeInfo.IsGenericTypeDefinition
                    ? typeInfo.GenericTypeParameters.Select(t => t.GetReadableName())
                    : type.GenericTypeArguments.Select(t => t.GetReadableName()));

                // Determine if could retrieve at least one generic argument
                if (genericArgs.Count > 0)
                {
                    // We could retrieve at least one generic argument
                    sb.Append('{')
                      .Append(string.Join(",", genericArgs))
                      .Append('}');
                }
            }

            // Done
            sb.Insert(0, name);
            return sb.ToString();
        }

        private static bool HasSameGenericTypeDefinition(TypeInfo type, TypeInfo otherType)
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

        private static bool TypeImplementsInterface(TypeInfo type, TypeInfo otherType)
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
                       .Any(t => Equals(t, otherType) || isGeneric && HasSameGenericTypeDefinition(t, otherType));
        }

        private static bool TypeInheritClass(TypeInfo type, TypeInfo otherType)
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
                if (Equals(baseClrType, otherType) || isGeneric && HasSameGenericTypeDefinition(baseClrType, otherType))
                    return true;

                baseType = baseClrType.BaseType;
            }

            // The type doesn't meet the criteria
            return false;
        }

    }

}