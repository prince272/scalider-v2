#region # using statements #

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using Scalider.Data.Entities;
using Scalider.Data.UnitOfWork;

#endregion

namespace Scalider.Data.Repository
{

    /// <summary>
    /// Provides an implementation of the <see cref="IRepository{TEntity}"/>
    /// generic interface that uses Entity Framework to read and write on the
    /// database.
    /// </summary>
    /// <typeparam name="TContext">The type encapsulating the database
    /// context.</typeparam>
    /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
    public class EfRepository<TContext, TEntity> : BaseRepository<TEntity>
        where TContext : DbContext
        where TEntity : class, IEntity
    {

        #region # Variables #

        private DbSet<TEntity> _dbSet;

        #endregion

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="EfRepository{TContext, TEntity}"/> class.
        /// </summary>
        /// <param name="unitOfWork"></param>
        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public EfRepository([NotNull] EfUnitOfWork<TContext> unitOfWork)
        {
            Check.NotNull(unitOfWork, nameof(unitOfWork));
            Context = unitOfWork.Context;
        }

        #region # Properties #

        #region == Protected ==

        /// <summary>
        /// Gets the database context being used by the repository.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        protected TContext Context { get; }

        /// <summary>
        /// Gets the <see cref="DbSet{T}"/> being used by the repository.
        /// </summary>
        protected DbSet<TEntity> DbSet =>
            _dbSet ?? (_dbSet = Context.Set<TEntity>());

        #endregion

        #endregion

        #region # Methods #

        #region == Overrides ==

        /// <inheritdoc />
        public override int Count() => DbSet.Count();

        /// <inheritdoc />
        public override Task<int> CountAsync(
            CancellationToken cancellationToken) =>
            DbSet.CountAsync(cancellationToken);

        /// <inheritdoc />
        public override long LongCount() => DbSet.LongCount();

        /// <inheritdoc />
        public override Task<long> LongCountAsync(
            CancellationToken cancellationToken) =>
            DbSet.LongCountAsync(cancellationToken);

        /// <inheritdoc />
        public override int Count(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Count(predicate);
        }

        /// <inheritdoc />
        public override Task<int> CountAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.CountAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public override long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.LongCount(predicate);
        }

        /// <inheritdoc />
        public override Task<long> LongCountAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken) =>
            DbSet.LongCountAsync(predicate, cancellationToken);

        /// <inheritdoc />
        public override IEnumerable<TEntity> GetAll() => DbSet;

        /// <inheritdoc />
        public override Task<IEnumerable<TEntity>> GetAllAsync(
            CancellationToken cancellationToken) =>
            Task.FromResult(DbSet.AsAsyncEnumerable() as IEnumerable<TEntity>);

        /// <inheritdoc />
        public override IEnumerable<TEntity> Find(
            Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Where(predicate).ToList();
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            Check.NotNull(predicate, nameof(predicate));
            return await DbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Single(predicate);
        }

        /// <inheritdoc />
        public override Task<TEntity> SingleAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.SingleAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public override TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.First(predicate);
        }

        /// <inheritdoc />
        public override Task<TEntity> FirstAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.FirstAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public override TEntity FirstOrDefault(
            Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.FirstOrDefault(predicate);
        }

        /// <inheritdoc />
        public override Task<TEntity> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public override TEntity Last(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.Last(predicate);
        }

        /// <inheritdoc />
        public override Task<TEntity> LastAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.LastAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public override TEntity LastOrDefault(
            Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.LastOrDefault(predicate);
        }

        /// <inheritdoc />
        public override Task<TEntity> LastOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            Check.NotNull(predicate, nameof(predicate));
            return DbSet.LastOrDefaultAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public override void Add(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Add(entity);
        }

        /// <inheritdoc />
        public override Task AddAsync(TEntity entity,
            CancellationToken cancellationToken)
        {
            Check.NotNull(entity, nameof(entity));
            return DbSet.AddAsync(entity, cancellationToken);
        }

        /// <inheritdoc />
        public override void Update(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Update(entity);
        }

        /// <inheritdoc />
        public override Task UpdateAsync(TEntity entity,
            CancellationToken cancellationToken)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Update(entity);

            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public override void Remove(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Remove(entity);
        }

        /// <inheritdoc />
        public override Task RemoveAsync(TEntity entity,
            CancellationToken cancellationToken)
        {
            Check.NotNull(entity, nameof(entity));
            DbSet.Remove(entity);

            return Task.FromResult(0);
        }

        #endregion

        #endregion

    }
}