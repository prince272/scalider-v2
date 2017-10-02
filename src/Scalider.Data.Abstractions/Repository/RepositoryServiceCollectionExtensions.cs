#region # using statements #

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

#endregion

namespace Scalider.Data.Repository
{

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> 
    /// interface.
    /// </summary>
    public static class RepositoryServiceCollectionExtensions
    {

        /// <summary>
        /// Scans an assembly for types that implement the
        /// <see cref="IRepository"/> interface, wether it be directly or via
        /// inheritance, and adds the found types as services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that
        /// services should be added to.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to scan for
        /// repositories.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddAssemblyRepositories(
            [NotNull] this IServiceCollection services, [NotNull] Assembly assembly)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(assembly, nameof(assembly));

            // Retrieve all the types defined by the assembly
            Type[] assemblyTypes;
            try
            {
                assemblyTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException tlException)
            {
                assemblyTypes = tlException.Types;
            }

            // Keep only the repositories
            var repositoryTypes = assemblyTypes
                .Where(t => t != null)
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType)
                .Where(t => t.GetInterfaces().Contains(typeof(IRepository)));

            // Register all the repositories
            foreach (var type in repositoryTypes)
            {
                services.TryAddScoped(type, type);

                // Retrieve all the repository definitions
                var interfaces = type.GetInterfaces().Where(i => i != null);
                foreach (var @interface in interfaces)
                {
                    if (!@interface.IsGenericType &&
                        @interface.GetInterfaces().Contains(typeof(IRepository)))
                    {
                        // Repository definition found, add as a service
                        services.TryAddScoped(@interface, type);
                    }

                }
            }

            // Done
            return services;
        }

    }
}