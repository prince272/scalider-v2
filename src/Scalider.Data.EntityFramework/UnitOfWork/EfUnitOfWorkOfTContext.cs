#region # using statements #

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Scalider.Data.UnitOfWork
{

    /// <summary>
    /// Provides an implementation of the <see cref="IUnitOfWork"/> interface
    /// that uses Entity Framework to access the database.
    /// </summary>
    /// <typeparam name="TContext">The type encapsulating the database
    /// context.</typeparam>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class EfUnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EfUnitOfWork{TContext}"/>
        /// class.
        /// </summary>
        /// <param name="context"></param>
        public EfUnitOfWork([NotNull] TContext context)
        {
            context.Database.AutoTransactionsEnabled = false;
            Context = context;
        }

        #region # Properties #

        #region == Protected Internal ==

        /// <summary>
        /// Gets the <see cref="DbContext"/> being used by the unit of work.
        /// </summary>
        protected internal TContext Context { get; }

        #endregion

        #endregion

        #region # IDisposable #

        /// <inheritdoc />
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region # IUnitOfWork #

        /// <inheritdoc />
        public IUnitOfWorkTransaction CurrentTransaction =>
            Context.Database.CurrentTransaction == null
                ? null
                : new EfUnitOfWorkTransaction(Context.Database.CurrentTransaction);

        /// <inheritdoc />
        public IUnitOfWorkTransaction BeginTransaction() =>
            new EfUnitOfWorkTransaction(Context.Database.BeginTransaction());

        /// <inheritdoc />
        public Task<IUnitOfWorkTransaction> BeginTransactionAsync() =>
            BeginTransactionAsync(CancellationToken.None);

        /// <inheritdoc />
        public Task<IUnitOfWorkTransaction>
            BeginTransactionAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IUnitOfWorkTransaction>(
                new EfUnitOfWorkTransaction(Context.Database.BeginTransaction()));

        /// <inheritdoc />
        public void CommitTransaction() => CurrentTransaction?.Commit();

        /// <inheritdoc />
        public void RollbackTransaction() => CurrentTransaction.Rollback();

        /// <inheritdoc />
        public int SaveChanges() => Context.SaveChanges();

        /// <inheritdoc />
        public Task<int> SaveChangesAsync() =>
            SaveChangesAsync(CancellationToken.None);

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
            Context.SaveChangesAsync(cancellationToken);

        #endregion

    }

}