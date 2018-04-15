using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Scalider.Reflection;

namespace Scalider.EntityFrameworkCore
{
    
    /// <summary>
    /// Provides extension methods for the <see cref="ModelBuilder"/> class.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        
#if NETSTANDARD2_0

        private static readonly Lazy<MethodInfo> ApplyConfigurationMethod = new Lazy<MethodInfo>(() =>
            typeof(ModelBuilder).GetTypeInfo().DeclaredMethods.First(t => t.Name == "ApplyConfiguration"));
    
#else

        private static readonly Lazy<MethodInfo> ApplyConfigurationMethod = new Lazy<MethodInfo>(() =>
            typeof(ModelBuilderExtensions).GetTypeInfo().DeclaredMethods
                                          .First(t => t.Name == nameof(ApplyConfiguration)));

#endif

        /// <summary>
        /// Scans the assembly of <typeparamref name="T"/> for types that implement the
        /// <see cref="IEntityTypeConfiguration{TEntity}"/> interface and applies them to the given
        /// <paramref name="modelBuilder"/>.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/>.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// The <see cref="ModelBuilder"/>.
        /// </returns>
        [UsedImplicitly]
        public static ModelBuilder ApplyConfigurationFromAssemblyOf<T>([NotNull] this ModelBuilder modelBuilder) =>
            ApplyConfigurationFromAssembly(modelBuilder, typeof(T).GetTypeInfo().Assembly);

        /// <summary>
        /// Scans an assembly for types that implement the <see cref="IEntityTypeConfiguration{TEntity}"/> interface
        /// and applies them to the given <paramref name="modelBuilder"/>.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/>.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to scan.</param>
        /// <returns>
        /// The <see cref="ModelBuilder"/>.
        /// </returns>
        [UsedImplicitly]
        public static ModelBuilder ApplyConfigurationFromAssembly([NotNull] this ModelBuilder modelBuilder,
            [NotNull] Assembly assembly)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(assembly, nameof(assembly));

            // Retrieve all the type configuration
            var interfaceType = typeof(IEntityTypeConfiguration<>);
            var assemblyTypes =
                assembly.GetTypesOf(interfaceType)
                        .Select(t => t.GetTypeInfo())
                        .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType);

            // Apply all the type configurations
            foreach (var type in assemblyTypes)
            {
                var configurationInterfaces =
                    type.GetInterfaces()
                        .Select(t => t.GetTypeInfo())
                        .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType);

                var instance = Activator.CreateInstance(type.AsType());
                foreach (var ci in configurationInterfaces)
                {
                    var args = ci.GetGenericArguments();

                    // Validate that the generic type is a class
                    var entityType = args[0].GetTypeInfo();
                    if (!entityType.IsClass)
                        continue;

                    // Apply the configuration
                    ApplyConfigurationMethod
                        .Value
                        .MakeGenericMethod(args[0])
#if NETSTANDARD2_0
                        .Invoke(modelBuilder, new[] {instance});
#else
                        .Invoke(null, new[] {modelBuilder, instance});
#endif
                }
            }

            // Done
            return modelBuilder;
        }

#if !NETSTANDARD2_0

        /// <summary>
        /// Applies configuration that is defined in an <see cref="IEntityTypeConfiguration{TEntity}" /> instance.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/>.</param>
        /// <param name="configuration">The configuration to be applied.</param>
        /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
        /// <returns>
        /// The same <see cref="ModelBuilder" /> instance so that additional configuration calls can be chained.
        /// </returns>
        public static ModelBuilder ApplyConfiguration<TEntity>([NotNull] this ModelBuilder modelBuilder,
            [NotNull] IEntityTypeConfiguration<TEntity> configuration)
            where TEntity : class
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(configuration, nameof(configuration));

            configuration.Configure(modelBuilder.Entity<TEntity>());
            return modelBuilder;
        }

#endif
        
    }
    
}