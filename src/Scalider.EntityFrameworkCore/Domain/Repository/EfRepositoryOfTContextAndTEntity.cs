using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Scalider.Domain.Entity;

namespace Scalider.Domain.Repository
{

    /// <summary>
    /// Implementation of the <see cref="IRepository{TEntity}"/> interface that uses Entity Framework Core.
    /// </summary>
    /// <typeparam name="TContext">The type encapsulating the database context.</typeparam>
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
        protected DbSet<TEntity> DbSet => Context.Set<TEntity>();

        #region IBatchRepository<TEntity> Members

        /// <inheritdoc />
        public virtual int Count() => DbSet.Count();

        /// <inheritdoc />
        public Task<int> CountAsync(CancellationToken cancellationToken) => DbSet.CountAsync(cancellationToken);

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> GetAll() => DbSet.ToList();

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await DbSet.ToListAsync(cancellationToken);

        /// <inheritdoc />
        public virtual void Add(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Add(entity);
        }

        /// <inheritdoc />
        public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Check.NotNull(entity, nameof(entity));
            return DbSet.AddAsync(entity, cancellationToken);
        }

        /// <inheritdoc />
        public virtual void Update(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Update(entity);
        }

        /// <inheritdoc />
        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Update(entity);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual void Remove(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Remove(entity);
        }

        /// <inheritdoc />
        public virtual Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Remove(entity);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            Check.NotNull(entities, nameof(entities));
            DbSet.AddRange(entities);
        }

        /// <inheritdoc />
        public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            Check.NotNull(entities, nameof(entities));
            return DbSet.AddRangeAsync(entities, cancellationToken);
        }

        /// <inheritdoc />
        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            Check.NotNull(entities, nameof(entities));
            DbSet.UpdateRange(entities);
        }

        /// <inheritdoc />
        public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(entities, nameof(entities));
            DbSet.UpdateRange(entities);
            
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            Check.NotNull(entities, nameof(entities));
            DbSet.RemoveRange(entities);
        }

        /// <inheritdoc />
        public virtual Task RemoveRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(entities, nameof(entities));
            DbSet.RemoveRange(entities);
            
            return Task.CompletedTask;
        }

        #endregion

    }
}