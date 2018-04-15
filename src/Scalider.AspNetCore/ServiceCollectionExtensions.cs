using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Scalider.AspNetCore
{

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> class.
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Adds the base <see cref="IViewRenderer"/> implementation as a service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that
        /// services should be added to.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddDefaultViewRenderer([NotNull] this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            services.AddScoped<IViewRenderer, DefaultViewRenderer>();
            return services;
        }

    }
}