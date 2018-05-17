using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Scalider.Data.Entity;

namespace Scalider.Data.Repository
{

    /// <summary>
    /// Provides an implementation of the <see cref="IRepository{TEntity}"/> generic interface that uses
    /// Entity Framework Core to read and write on the database.
    /// </summary>
    /// <typeparam name="TContext">The type encapsulating the database
    /// context.</typeparam>
    /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
    public class EfRepository<TContext, TEntity> : IBatchRepository<TEntity>
        where TContext : DbContext
        where TEntity : class, IEntity
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepository{TContext, TEntity}"/> class.
        /// </summary>
        /// <param name="context"></param>
        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public EfRepository([NotNull] TContext context)
        {
            Check.NotNull(context, nameof(context));

            Context = context;
        }

        /// <summary>
        /// Gets the database context being used by the repository.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        protected TContext Context { get; }

        /// <summary>
        /// Gets the <see cref="DbSet{T}"/> being used by the repository.
        /// </summary>
        [UsedImplicitly]
        protected Lazy<DbSet<TEntity>> DbSet => new Lazy<DbSet<TEntity>>(() => Context.Set<TEntity>());

        /// <summary>
        /// Returns a <see cref="IQueryable{T}"/> with all the navigations for the entity of the repository, if
        /// the entity defines any navigation.
        /// </summary>
        /// <returns>
        /// The <see cref="IQueryable{T}"/> with the included navigations (if any).
        /// </returns>
        [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
        protected virtual IQueryable<TEntity> GetQueryableWithIncludes()
        {
            IQueryable<TEntity> set = DbSet.Value;
            var entityType = Context.Model.FindEntityType(typeof(TEntity));
            if (entityType == null)
                return set;

            // Retrieve the navigations for the entity
            var navigations = entityType.GetNavigations()?.ToArray();
            if (navigations == null || navigations.Length == 0)
            {
                // The entity doesn't have any navigation
                return set;
            }

            // Done
            return navigations.Aggregate(set,
                (current, nav) => current.Include(nav.Name));
        }

        #region IBatchRepository<TEntity> Members

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> GetAll() =>
            GetQueryableWithIncludes().ToList();

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await GetQueryableWithIncludes().ToListAsync(cancellationToken);

        /// <inheritdoc />
        public virtual void Add(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Value.Add(entity);
        }

        /// <inheritdoc />
        public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Check.NotNull(entity, nameof(entity));
            return DbSet.Value.AddAsync(entity, cancellationToken);
        }

        /// <inheritdoc />
        public virtual void Update(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Value.Update(entity);
        }

        /// <inheritdoc />
        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Value.Update(entity);

            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public virtual void Remove(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Value.Remove(entity);
        }

        /// <inheritdoc />
        public virtual Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Value.Remove(entity);

            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            Check.NotNull(entities, nameof(entities));
            DbSet.Value.AddRange(entities);
        }

        /// <inheritdoc />
        public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            Check.NotNull(entities, nameof(entities));
            return DbSet.Value.AddRangeAsync(entities, cancellationToken);
        }

        /// <inheritdoc />
        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            Check.NotNull(entities, nameof(entities));
            DbSet.Value.UpdateRange(entities);
        }

        /// <inheritdoc />
        public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(entities, nameof(entities));
            DbSet.Value.UpdateRange(entities);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            Check.NotNull(entities, nameof(entities));
            DbSet.Value.RemoveRange(entities);
        }

        /// <inheritdoc />
        public virtual Task RemoveRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(entities, nameof(entities));
            DbSet.Value.RemoveRange(entities);
            return Task.CompletedTask;
        }

        #endregion

    }
}