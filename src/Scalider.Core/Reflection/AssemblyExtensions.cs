using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Scalider.Reflection
{
    
    /// <summary>
    /// Provides extension methods for the <see cref="Assembly"/> class.
    /// </summary>
    public static class AssemblyExtensions
    {

        private static readonly ConcurrentDictionary<string, IEnumerable<Type>>
            TypesCache = new ConcurrentDictionary<string, IEnumerable<Type>>();

        public static void ClearCachedTypes() => TypesCache.Clear();

        /// <summary>
        /// Retrieves all the available types from an assembly. A type may not be available due to a missing reference.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to retrieve
        /// types from.</param>
        /// <returns>
        /// An array containing all the available types from the given <paramref name="assembly"/>.
        /// </returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static IEnumerable<Type> GetAvailableTypes([NotNull] this Assembly assembly)
        {
            Check.NotNull(assembly, nameof(assembly));
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

        public static IEnumerable<Type> GetTypesOf<T>([NotNull] this Assembly assembly) =>
            GetTypesOf(assembly, typeof(T));

        [UsedImplicitly]
        public static IEnumerable<Type> GetTypesOf([NotNull] this Assembly assembly, [NotNull] Type requiredType)
        {
            Check.NotNull(assembly, nameof(assembly));
            Check.NotNull(requiredType, nameof(requiredType));

            // Retrieve the assembly types
            var assemblyTypes = GetAvailableTypes(assembly).ToArray();
            
            if (!assemblyTypes.Any())
                return Enumerable.Empty<Type>();

            // Retrieve all the filters that implement or extends the required type
            return from t in assemblyTypes
                   where t.ImplementsOrInherits(requiredType)
                   select t;
        }

    }
    
}