using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Scalider;
using Scalider.Hosting.Queue;
using Scalider.Hosting.Schedule;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Registers the <see cref="TaskQueueHostedService"/> hosted service with the default
        /// <see cref="ITaskQueueService"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that services should be added to.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
        public static IServiceCollection AddTaskQueueService([NotNull] this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            services.AddTransient<IHostedService, TaskQueueHostedService>();
            services.TryAddSingleton<ITaskQueueService, DefaultTaskQueueService>();

            return services;
        }

        /// <summary>
        /// Registers the <see cref="TaskScheduleHostedService"/> hosted service with the default
        /// <see cref="ITaskSchedulerService"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that services should be added to.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
        public static IServiceCollection AddTaskScheduleService([NotNull] this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            services.AddTransient<IHostedService, TaskScheduleHostedService>();
            services.TryAddSingleton<ITaskSchedulerService, DefaultTaskSchedulerService>();

            return services;
        }

    }
    
}