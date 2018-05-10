using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Scalider.Identity;

namespace Scalider.Identity
{
    
    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Register the <see cref="BCryptPasswordHasher{TUser}"/> with the default options as a service.
        /// </summary>
        /// <typeparam name="TUser">The type used to represent a user.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> that services should be added to.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        [UsedImplicitly]
        public static IServiceCollection AddBCryptPasswordHasher<TUser>([NotNull] this IServiceCollection services)
            where TUser : class
        {
            Check.NotNull(services, nameof(services));

            // Register services
            services.AddSingleton<IPasswordHasher<TUser>, BCryptPasswordHasher<TUser>>();
            
            // Done
            return services;
        }

        /// <summary>
        /// Register the <see cref="BCryptPasswordHasher{TUser}"/> custom options as a service with
        /// </summary>
        /// <typeparam name="TUser">The type used to represent a user.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> that services should be added to.</param>
        /// <param name="configureAction"></param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddBCryptPasswordHasher<TUser>([NotNull] this IServiceCollection services,
            Action<BCryptPasswordHasherOptions> configureAction)
            where TUser : class
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(configureAction, nameof(configureAction));

            // Register services
            AddBCryptPasswordHasher<TUser>(services);
            services.Configure(configureAction);
            
            // Done
            return services;
        }
        
    }
}