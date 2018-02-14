using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Scalider.Data.Entity;

namespace Scalider.Data.Repository
{

    /// <summary>
    /// Provides extension methods for the <see cref="IEfRepository{TEntity}"/>
    /// interface.
    /// </summary>
    public static class EfRepositoryExtensions
    {

        /// <summary>
        /// Adds a collection of new entities to the data store.
        /// </summary>
        /// <param name="repository">The
        /// <see cref="IEfRepository{TEntity}"/></param>
        /// <param name="entities">The collection of entities to add to the
        /// data store.</param>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        public static void AddRange<TEntity>(
            [NotNull] this IEfRepository<TEntity> repository,
            [NotNull] params TEntity[] entities)
            where TEntity : class, IEntity
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(entities, nameof(entities));

            repository.AddRange(entities.AsEnumerable());
        }

        /// <summary>
        /// Asynchronously adds a collection of new entities to the data store.
        /// </summary>
        /// <param name="repository">The
        /// <see cref="IEfRepository{TEntity}"/></param>
        /// <param name="entities">The collection of entities to add to the
        /// data store.</param>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <returns></returns>
        public static Task AddRangeAsync<TEntity>(
            [NotNull] this IEfRepository<TEntity> repository,
            [NotNull] params TEntity[] entities)
            where TEntity : class, IEntity
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(entities, nameof(entities));

            return repository.AddRangeAsync(entities.AsEnumerable());
        }

    }
}