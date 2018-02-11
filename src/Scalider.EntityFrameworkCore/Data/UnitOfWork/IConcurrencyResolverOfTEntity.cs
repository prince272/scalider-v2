using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Scalider.Data.Entity;

namespace Scalider.Data.UnitOfWork
{
    
    /// <summary>
    /// Defines the basic functionality of a concurrency resolver.
    /// </summary>
    /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
    public interface IConcurrencyResolver<in TEntity>
        where TEntity : IEntity
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityEntry"></param>
        /// <param name="entity"></param>
        /// <param name="original"></param>
        /// <param name="resolved"></param>
        void Resolve([NotNull] EntityEntry entityEntry, [NotNull] TEntity entity,
            [NotNull] TEntity original, [NotNull] TEntity resolved);

    }
    
}