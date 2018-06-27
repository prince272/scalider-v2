using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalider;
using Scalider.MailKit;
using Scalider.Net.Mail;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class MailKitServiceCollectionExtensions
    {

        /// <summary>
        /// Registers the <see cref="MailKitEmailSender"/> as a service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that services should be added to.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddMailKitEmailSender([NotNull] this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));
            
            services.TryAddSingleton(typeof(IEmailSender), typeof(MailKitEmailSender));
            return services;
        }
        
        /// <summary>
        /// Registers the <see cref="MailKitEmailSender"/> as a service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that services should be added to.</param>
        /// <param name="configureOptions">The action used to configure the MailKit options.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddMailKitEmailSender([NotNull] this IServiceCollection services,
            Action<MailKitEmailSenderOptions> configureOptions)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(configureOptions, nameof(configureOptions));

            services.Configure(configureOptions);
            services.TryAddSingleton(typeof(IEmailSender), typeof(MailKitEmailSender));

            return services;
        }

    }
}