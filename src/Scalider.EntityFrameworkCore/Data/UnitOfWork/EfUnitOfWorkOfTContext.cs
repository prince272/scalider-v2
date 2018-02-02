using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Scalider.Data.UnitOfWork
{

    /// <summary>
    /// Provides an implementation of the <see cref="IUnitOfWork"/> interface
    /// that uses Entity Framework Core.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class EfUnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {

        private readonly TContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="EfUnitOfWork{TContext}"/> class.
        /// </summary>
        /// <param name="dbContext"></param>
        public EfUnitOfWork([NotNull] TContext dbContext)
        {
            Check.NotNull(dbContext, nameof(dbContext));

            _dbContext = dbContext;
        }

        #region IUnitOfWork Members

        /// <inheritdoc />
        public virtual void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        /// <inheritdoc />
        public virtual Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            // no-op
        }

        #endregion

    }

}