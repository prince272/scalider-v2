using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Scalider.Domain.UnitOfWork
{

    /// <summary>
    /// Implementation of the <see cref="IUnitOfWork"/> interface that uses Entity Framework Core.
    /// </summary>
    /// <typeparam name="TContext">The type encapsulating the database context.</typeparam>
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class EfUnitOfWork<TContext> : IEfUnitOfWork
        where TContext : DbContext
    {

        private readonly TContext _dbContext;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EfUnitOfWork{TContext}"/> class.
        /// </summary>
        /// <param name="dbContext"></param>
        public EfUnitOfWork([NotNull] TContext dbContext)
        {
            Check.NotNull(dbContext, nameof(dbContext));

            _dbContext = dbContext;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value indicating whether the dispose method was called.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }

        #region IEfUnitOfWork Members

        /// <inheritdoc />
        DbContext IEfUnitOfWork.DbContext => _dbContext;

        /// <inheritdoc />
        public virtual void SaveChanges() => _dbContext.SaveChanges();

        /// <inheritdoc />
        public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _dbContext.SaveChangesAsync(cancellationToken);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

}