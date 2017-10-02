#region # using statements #

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalider.Data.UnitOfWork;

#endregion

namespace Scalider.Data
{

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> 
    /// interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Register the <see cref="EfUnitOfWork{TContext}"/> as a service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that
        /// services should be added to.</param>
        /// <typeparam name="TContext">The type encapsulating the database
        /// context.</typeparam>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddEfUnitOfWork<TContext>(
            [NotNull] this IServiceCollection services)
            where TContext : DbContext
        {
            Check.NotNull(services, nameof(services));

            // Register services
            services.AddScoped<IUnitOfWork, EfUnitOfWork<TContext>>()
                    .AddScoped<EfUnitOfWork<TContext>>();

            // Done
            return services;
        }

    }
}