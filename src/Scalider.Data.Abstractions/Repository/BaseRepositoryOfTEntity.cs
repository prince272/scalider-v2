#region # using statements #

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Scalider.Data.Entities;

#endregion

namespace Scalider.Data.Repository
{

    /// <summary>
    /// Provides a base class for implementations of the
    /// <see cref="IRepository{TEntity}"/> generic interface.
    /// </summary>
    /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{TEntity}"/>
        /// class.
        /// </summary>
        protected BaseRepository()
        {
        }

        #region # IRepository #

        /// <inheritdoc />
        public abstract int Count();

        /// <inheritdoc />
        public Task<int> CountAsync() => CountAsync(CancellationToken.None);

        /// <inheritdoc />
        public abstract Task<int> CountAsync(CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract long LongCount();

        /// <inheritdoc />
        public Task<long> LongCountAsync() => LongCountAsync(CancellationToken.None);

        /// <inheritdoc />
        public abstract Task<long> LongCountAsync(
            CancellationToken cancellationToken);

        #endregion

        #region # IRepository<TEntity> #

        /// <inheritdoc />
        public abstract int Count(Expression<Func<TEntity, bool>> predicate);

        /// <inheritdoc />
        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return CountAsync(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task<int> CountAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract long LongCount(Expression<Func<TEntity, bool>> predicate);

        /// <inheritdoc />
        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return LongCountAsync(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task<long> LongCountAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract IEnumerable<TEntity> GetAll();

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> GetAllAsync() =>
            GetAllAsync(CancellationToken.None);

        /// <inheritdoc />
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(
            CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract IEnumerable<TEntity> Find(
            Expression<Func<TEntity, bool>> predicate);

        /// <inheritdoc />
        public Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return FindAsync(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract TEntity Single(Expression<Func<TEntity, bool>> predicate);

        /// <inheritdoc />
        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return SingleAsync(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task<TEntity> SingleAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract TEntity First(Expression<Func<TEntity, bool>> predicate);

        /// <inheritdoc />
        public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return FirstAsync(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task<TEntity> FirstAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract TEntity FirstOrDefault(
            Expression<Func<TEntity, bool>> predicate);

        /// <inheritdoc />
        public Task<TEntity> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return FirstOrDefaultAsync(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task<TEntity> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract TEntity Last(Expression<Func<TEntity, bool>> predicate);

        /// <inheritdoc />
        public Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return LastAsync(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task<TEntity> LastAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract TEntity LastOrDefault(
            Expression<Func<TEntity, bool>> predicate);

        /// <inheritdoc />
        public Task<TEntity> LastOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            return LastOrDefaultAsync(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task<TEntity> LastOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract void Add(TEntity entity);

        /// <inheritdoc />
        public Task AddAsync(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            return AddAsync(entity, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task AddAsync(TEntity entity,
            CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract void Update(TEntity entity);

        /// <inheritdoc />
        public Task UpdateAsync(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            return UpdateAsync(entity, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task UpdateAsync(TEntity entity,
            CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract void Remove(TEntity entity);

        /// <inheritdoc />
        public Task RemoveAsync(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));
            return RemoveAsync(entity, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task RemoveAsync(TEntity entity,
            CancellationToken cancellationToken);

        #endregion

    }
}