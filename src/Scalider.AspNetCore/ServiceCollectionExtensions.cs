using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalider;
using Scalider.AspNetCore;
using Scalider.Template;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> class.
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Registers the <see cref="RazorTemplateRenderer"/> as a service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        [UsedImplicitly]
        public static IServiceCollection AddRazorTemplateRenderer([NotNull] this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            services.TryAddScoped<ITemplateRenderer, RazorTemplateRenderer>();
            return services;
        }

        /// <summary>
        /// Registers the default <see cref="IWebProxyHelper"/> as a service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        [UsedImplicitly]
        public static IServiceCollection AddDefaultWebProxyHelper([NotNull] this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            services.TryAddSingleton<IWebProxyHelper, DefaultWebProxyHelper>();
            return services;
        }

    }
}