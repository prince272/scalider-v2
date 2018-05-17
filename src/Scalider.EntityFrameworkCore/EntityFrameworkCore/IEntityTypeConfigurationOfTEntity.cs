#if !NETSTANDARD2_0
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scalider.EntityFrameworkCore
{

    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate
    /// class, rather than in-line in
    /// <see cref="M:DbContext.OnModelCreating(ModelBuilder)" />.
    /// 
    /// Implement this interface, applying configuration for the entity in the
    /// <see cref="M:IEntityTypeConfiguration{TEntity}.Configure(EntityTypeBuilder{TEntity})" />
    /// method, and then apply the configuration to the model using the
    /// extension method
    /// <see cref="M:ModelBuilderExtensions.ApplyConfiguration{TEntity}(Microsoft.EntityFrameworkCore.ModelBuilder,IEntityTypeConfiguration{TEntity})" />
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntityTypeConfiguration<TEntity>
        where TEntity : class
    {

        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the
        /// entity type.</param>
        void Configure([NotNull] EntityTypeBuilder<TEntity> builder);

    }

}
#endif