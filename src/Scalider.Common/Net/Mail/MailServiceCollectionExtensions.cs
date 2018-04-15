#if NETSTANDARD2_0
using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalider.Net.Mail.Smtp;

namespace Scalider.Net.Mail
{
    
    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class MailServiceCollectionExtensions
    {

        /// <summary>
        /// Registers the <see cref="NullEmailSender"/> as a service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that services should be added to.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddNullEmailSender([NotNull] this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            services.TryAddSingleton(typeof(IEmailSender), typeof(NullEmailSender));
            return services;
        }

        /// <summary>
        /// Registers the <see cref="SmtpEmailSender"/> as a service.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> that services should be added to.</param>
        /// <param name="configureOptions">The action used to configure the SMTP options.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddSmtpEmailSender([NotNull] this IServiceCollection services,
            [NotNull] Action<SmtpEmailSenderOptions> configureOptions)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(configureOptions, nameof(configureOptions));

            services.Configure(configureOptions);
            services.TryAddSingleton<IEmailSender, SmtpEmailSender>();

            return services;
        }
        
    }
    
}
#endif