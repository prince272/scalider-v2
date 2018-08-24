using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Scalider.Domain.Repository
{

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class RepositoryServiceCollectionExtensions
    {

        /// <summary>
        /// Scans a assembly of <typeparamref name="T"/> for types that implement the <see cref="IRepository"/>
        /// interface, whether it be directly or via inheritance, and adds the found types as services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that services should be added to.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        [UsedImplicitly]
        public static IServiceCollection AddRepositoriesFromAssemblyOf<T>(
            [NotNull] this IServiceCollection services) => AddRepositoriesFromAssembly(services, typeof(T).Assembly);

        /// <summary>
        /// Scans an assembly for types that implement the <see cref="IRepository"/> interface, wether it be directly
        /// or via inheritance, and adds the found types as services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that services should be added to.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to scan for repositories.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        [UsedImplicitly]
        public static IServiceCollection AddRepositoriesFromAssembly([NotNull] this IServiceCollection services,
            [NotNull] Assembly assembly)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(assembly, nameof(assembly));

            // Retrieve all the types exported by the assembly and filter out the types that aren't repositories
            var repositoryInterface = typeof(IRepository);
            var foundRepositoryTypes =
                ReflectionUtils.GetExportedTypes(assembly)
                               .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType)
                               .Where(t => repositoryInterface.IsAssignableFrom(t))
                               .ToArray();

            // Register all the repositories
            foreach (var implementationType in foundRepositoryTypes)
            {
                services.TryAddTransient(implementationType, implementationType);

                var typeInterfaces = implementationType.GetInterfaces().Where(t => t != null).ToArray();
                foreach (var intrfc in typeInterfaces)
                {
                    bool isRepositoryInterface = repositoryInterface.IsAssignableFrom(intrfc);
                    if (!isRepositoryInterface || intrfc.IsGenericType || intrfc == repositoryInterface)
                    {
                        // We don't care about interfaces that doesn't extends the base repository interface,
                        // that are generic types and we don't care about adding the base repository
                        // interface type as a service
                        continue;
                    }

                    services.TryAddTransient(intrfc, implementationType);
                }
            }

            // Done
            return services;
        }

    }
}