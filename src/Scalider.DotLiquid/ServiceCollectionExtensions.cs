using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalider;
using Scalider.DotLiquid;
using Scalider.Template;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Registers the <see cref="DotLiquidTemplateRenderer"/> as a service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddDotLiquidTemplateRenderer([NotNull] this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            services.TryAddSingleton<IHashConverter, ReflectionHashConverter>();
            services.TryAddSingleton<ITemplateRenderer, DotLiquidTemplateRenderer>();

            return services;
        }
        
    }
}