using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace Scalider
{

    internal static class ReflectionUtils
    {

        private static readonly ConcurrentDictionary<string, IEnumerable<Type>>
            TypesCache = new ConcurrentDictionary<string, IEnumerable<Type>>();
        
        #region GetAvailableTypesFromAssembly

        /// <summary>
        /// Retrieves all the available types from an assembly. A type may not be available due to
        /// a missing reference.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to retrieve
        /// types from.</param>
        /// <param name="forceReload">A value indicating whether the cache should be invalidated for the
        /// assembly.</param>
        /// <returns>
        /// An array containing all the available types from the given <paramref name="assembly"/>.
        /// </returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static IEnumerable<Type> GetAvailableTypesFromAssembly([NotNull] Assembly assembly,
            bool forceReload = false)
        {
            Check.NotNull(assembly, nameof(assembly));
            if (forceReload)
            {
                // Try to remove the cache entry for the assembly
                TypesCache.TryRemove(assembly.FullName, out _);
            }
            
            // Retrieve or add a cache entry for the assembly
            return TypesCache.GetOrAdd(
                assembly.FullName,
                _ =>
                {
                    // Retrieve all the available types from the assembly
                    Type[] assemblyTypes;
                    try
                    {
                        assemblyTypes = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException tlException)
                    {
                        assemblyTypes = tlException.Types;
                    }

                    // Done
                    return assemblyTypes.Where(t => t != null);
                }
            );
        }
        
        #endregion

        [UsedImplicitly]
        public static string GetAssemblyName([NotNull] Assembly assembly)
        {
            Check.NotNull(assembly, nameof(assembly));

            var name = assembly.GetName(true);
            return name.Name;
        }
        
        #region GetTypeReadableName

        /// <summary>
        /// Gets the readable name for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        /// A string representing the readable name of the <paramref name="type"/>.
        /// </returns>
        public static string GetTypeReadableName([NotNull] Type type)
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
                    ? typeInfo.GenericTypeParameters.Select(GetTypeReadableName)
                    : type.GenericTypeArguments.Select(GetTypeReadableName));

                // Determine if could retrieve at least one generic argument
                if (genericArgs.Count > 0)
                {
                    // We could retrieve at least one generic argument
                    sb.Append('<')
                      .Append(string.Join(",", genericArgs))
                      .Append('>');
                }
            }

            // Done
            sb.Insert(0, name);
            return sb.ToString();
        }
        
        #endregion

        public static string GetTypeDisplayName([NotNull] Type type)
        {
            Check.NotNull(type, nameof(type));

            return $"{type.Namespace}.{GetTypeReadableName(type)} ({GetAssemblyName(type.Assembly)})";
        }

        public static string GetMemberDisplayName([NotNull] MemberInfo memberInfo)
        {
            Check.NotNull(memberInfo, nameof(memberInfo));

            var type = memberInfo.DeclaringType;
            return type == null
                ? $"<Unknown>.{memberInfo.Name} (<Unknown>)"
                : $"{type.Namespace}.{GetTypeReadableName(type)}.{memberInfo.Name} " +
                  $"({GetAssemblyName(type.Assembly)})";
        }

        private static IEnumerable<Type> GetAllInheritedTypes(Type type, bool includingSelf)
        {
            var result = new Stack<Type>();
            if (includingSelf)
                result.Push(type);

            // Navigate through all the base types until we reach the root
            var baseType = type.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                result.Push(baseType);
                baseType = baseType.BaseType;
            }
            
            // Done
            return result.ToArray();
        }
        
        #region IsAssignableFrom

        public static bool IsAssignableFrom(Type expectedType, Type typeToTest)
        {
            if (expectedType.IsAssignableFrom(typeToTest))
            {
                // The type to test is assignable to the expected type
                return true;
            }

            if (!expectedType.IsGenericTypeDefinition)
            {
                // The expected type isn't a type with a generic definition
                return false;
            }

            // Retrieve a collection containing all the types we need to validate and determine if any
            // is assignable to the expected type
            var typesToExplore = expectedType.IsInterface 
                ? typeToTest.GetInterfaces().Where(t => t != null)
                : GetAllInheritedTypes(typeToTest, false);

            return typesToExplore.Any(t => t.IsGenericType &&
                                           expectedType.IsAssignableFrom(t.GetGenericTypeDefinition()));
        }
        
        #endregion
        
        #region IsInstanceOfType

        public static bool IsInstanceOfType(Type expectedType, object obj)
        {
            if (expectedType.IsInstanceOfType(obj))
            {
                // The object is an instance of the expected type
                return true;
            }

            if (!expectedType.IsGenericTypeDefinition)
            {
                // The expected type isn't a type with a generic definition
                return false;
            }

            // Retrieve a collection containing all the types we need to validate and determine if any
            // is assignable to the expected type
            var objType = obj.GetType();
            var typesToExplore = expectedType.IsInterface 
                ? objType.GetInterfaces().Where(t => t != null)
                : GetAllInheritedTypes(objType, false);
            
            return typesToExplore.Any(t => t.IsGenericType &&
                                           // expectedType.IsAssignableFrom(t) &&
                                           expectedType.IsAssignableFrom(t.GetGenericTypeDefinition()));
        }
        
        #endregion
        
    }
    
}