﻿#if NETSTANDARD2_0
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

        private static Lazy<MethodInfo> ApplyConfigurationMethod =
            new Lazy<MethodInfo>(() =>
                typeof(ModelBuilder)
                    .GetTypeInfo().DeclaredMethods
                    .First(t => t.Name == "ApplyConfiguration"));
        
        /// <summary>
        /// Scans an assembly for types that implement the
        /// <see cref="IEntityTypeConfiguration{TEntity}"/> interface and
        /// applies them to the given <paramref name="modelBuilder"/>.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/>.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to scan.</param>
        /// <returns>
        /// The <see cref="ModelBuilder"/>.
        /// </returns>
        public static ModelBuilder ApplyConfigurationFromAssembly(
            [NotNull] ModelBuilder modelBuilder, [NotNull] Assembly assembly)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(assembly, nameof(assembly));
            
            // Retrieve all the type configuration
            var interfaceType = typeof(IEntityTypeConfiguration<>);
            var assemblyTypes =
                assembly.GetTypesOf(interfaceType)
                        .Select(t => t.GetTypeInfo())
                        .Where(t => t.IsClass && !t.IsAbstract && t.IsGenericType);
            
            // Apply all the type configurations
            foreach (var type in assemblyTypes)
            {
                var configurationInterfaces =
                    type.GetInterfaces()
                        .Where(t =>
                            t.IsGenericType && t.GetGenericTypeDefinition() ==
                            interfaceType);
                
                var instance = Activator.CreateInstance(type);
                foreach (var ci in configurationInterfaces)
                {
                    var args = ci.GetGenericArguments();
                    
                    // Validate that the generic type is a class
                    var entityType = args[0];
                    if (!entityType.IsClass)
                        continue;
                    
                    // Apply the configuration
                    ApplyConfigurationMethod
                        .Value
                        .MakeGenericMethod(args[0])
                        .Invoke(modelBuilder, new[] {instance});
                }
            }
            
            // Done
            return modelBuilder;
        }
        
    }
    
}
#endif