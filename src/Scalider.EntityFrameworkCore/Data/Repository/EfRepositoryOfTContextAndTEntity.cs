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
    /// Provides an implementation of the <see cref="IRepository{TEntity}"/>
    /// generic interface that uses Entity Framework Core to read and write on
    /// the database.
    /// </summary>
    /// <typeparam name="TContext">The type encapsulating the database
    /// context.</typeparam>
    /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
    public class EfRepository<TContext, TEntity> : IRepository<TEntity>
        where TContext : DbContext
        where TEntity : class, IEntity
    {

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="EfRepository{TContext, TEntity}"/> class.
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
        protected Lazy<DbSet<TEntity>> DbSet =>
            new Lazy<DbSet<TEntity>>(() => Context.Set<TEntity>());

        #region IRepository<TEntity> Members

        /*// <inheritdoc />
        public virtual int Count() => DbSet.Value.Count();

        /// <inheritdoc />
        public virtual Task<int> CountAsync(
            CancellationToken cancellationToken = default) =>
            DbSet.Value.CountAsync(cancellationToken);

        /// <inheritdoc />
        public virtual long LongCount() => DbSet.Value.LongCount();

        /// <inheritdoc />
        public virtual Task<long> LongCountAsync(
            CancellationToken cancellationToken = default) =>
            DbSet.Value.LongCountAsync(cancellationToken);

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Value.Count(predicate);
        }

        /// <inheritdoc />
        public virtual Task<int> CountAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Value.CountAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Value.LongCount(predicate);
        }

        /// <inheritdoc />
        public virtual Task<long> LongCountAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default) =>
            DbSet.Value.LongCountAsync(predicate, cancellationToken);*/

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> GetAll() => DbSet.Value.ToList();

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default) =>
            await DbSet.Value.ToListAsync(cancellationToken);

        /*// <inheritdoc />
        public virtual IEnumerable<TEntity> Find(
            Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Value.Where(predicate).ToList();
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(predicate, nameof(predicate));
            return await DbSet.Value.Where(predicate).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Value.Single(predicate);
        }

        /// <inheritdoc />
        public virtual Task<TEntity> SingleAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Value.SingleAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Value.First(predicate);
        }

        /// <inheritdoc />
        public virtual Task<TEntity> FirstAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Value.FirstAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual TEntity FirstOrDefault(
            Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Value.FirstOrDefault(predicate);
        }

        /// <inheritdoc />
        public virtual Task<TEntity> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Value.FirstOrDefaultAsync(predicate, cancellationToken);
        }*/

        /// <inheritdoc />
        public virtual void Add(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Value.Add(entity);
        }

        /// <inheritdoc />
        public virtual Task AddAsync(TEntity entity,
            CancellationToken cancellationToken = default)
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
        public virtual Task UpdateAsync(TEntity entity,
            CancellationToken cancellationToken = default)
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
        public virtual Task RemoveAsync(TEntity entity,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Value.Remove(entity);

            return Task.FromResult(0);
        }

        #endregion

    }
}