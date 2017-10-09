using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Scalider.Password
{
    
    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> 
    /// interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Register the <see cref="BCryptPasswordHasher"/> with the default options
        /// as a service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that
        /// services should be added to.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        [UsedImplicitly]
        public static IServiceCollection AddBCryptPasswordHasher(
            [NotNull] this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            // Register services
            services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            
            // Done
            return services;
        }

        /// <summary>
        /// Register the <see cref="BCryptPasswordHasher"/> custom options as a
        /// service with
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that
        /// services should be added to.</param>
        /// <param name="configureAction"></param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddBCryptPasswordHasher(
            [NotNull] this IServiceCollection services,
            Action<BCryptPasswordHasherOptions> configureAction)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(configureAction, nameof(configureAction));

            // Register services
            AddBCryptPasswordHasher(services);
            services.Configure(configureAction);
            
            // Done
            return services;
        }
        
    }
}