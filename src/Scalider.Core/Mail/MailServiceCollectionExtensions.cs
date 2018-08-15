using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalider;
using Scalider.Mail;
using Scalider.Mail.Smtp;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Provides extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class MailServiceCollectionExtensions
    {

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