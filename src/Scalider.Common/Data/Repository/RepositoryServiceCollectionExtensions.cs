using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalider.Reflection;

namespace Scalider.Data.Repository
{

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> 
    /// interface.
    /// </summary>
    public static class RepositoryServiceCollectionExtensions
    {

        /// <summary>
        /// Scans a assembly of <typeparamref name="T"/> for types that
        /// implement the <see cref="IRepository"/> interface, wether it be
        /// directly or via inheritance, and adds the found types as services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that
        /// services should be added to.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        [UsedImplicitly]
        public static IServiceCollection
            AddRepositoriesFromAssemblyOf<T>(
                [NotNull] this IServiceCollection services) =>
            AddRepositoriesFromAssembly(services, typeof(T).GetTypeInfo().Assembly);

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
        [UsedImplicitly]
        public static IServiceCollection AddRepositoriesFromAssembly(
            [NotNull] this IServiceCollection services, [NotNull] Assembly assembly)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(assembly, nameof(assembly));

            // Keep only the repositories
            var repositoryTypes =
                assembly.GetTypesOf<IRepository>()
                        .Select(t => t.GetTypeInfo())
                        .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType);

            // Register all the repositories
            foreach (var type in repositoryTypes)
            {
#if NETSTANDARD2_0
                services.TryAddScoped(type, type);
                AddAllInterfacesAsServicesForType(services, type, type);
#else
                var asType = type.AsType();
                services.TryAddScoped(asType, asType);
                AddAllInterfacesAsServicesForType(services, type, asType);
    #endif
            }

            // Done
            return services;
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static void AddAllInterfacesAsServicesForType(
            IServiceCollection services, TypeInfo type, Type implementationType)
        {
            var interfaces =
                type.GetInterfaces()
                    .Where(i => i != null)
                    .Select(i => new {clr = i.GetTypeInfo(), type = i});
                
            foreach (var @interface in interfaces)
            {
                var isRepositoryInterface =
                    @interface.clr.GetInterfaces().Contains(typeof(IRepository));
                
                if (@interface.clr.IsGenericType || !isRepositoryInterface)
                    continue;

                // Repository definition found, add as a service
                services.TryAddScoped(@interface.type, implementationType);
            }
        }

    }
}