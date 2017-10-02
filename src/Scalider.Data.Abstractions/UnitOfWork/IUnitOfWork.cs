#region # using statements #

using System;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Scalider.Data.UnitOfWork
{

    /// <summary>
    /// A unit of work represents a session with the database that can be used to
    /// save changes made to entities.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        
        /// <summary>
        /// Gets the current transaction being used by the unit of work, or null
        /// if no transaction is in use.
        /// </summary>
        IUnitOfWorkTransaction CurrentTransaction { get; }

        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        /// <returns>
        /// A <see cref="IUnitOfWorkTransaction"/> that represents the started
        /// transaction.
        /// </returns>
        IUnitOfWorkTransaction BeginTransaction();
        
        /// <summary>
        /// Asynchronously starts a new transaction.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous transaction initialization.
        /// </returns>
        Task<IUnitOfWorkTransaction> BeginTransactionAsync();

        /// <summary>
        /// Asynchronously starts a new transaction.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous transaction initialization.
        /// </returns>
        Task<IUnitOfWorkTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken);

        /// <summary>
        /// Applies the outstanding operations in the current transaction to the
        /// database.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Discards the outstanding operations in the current transaction.
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        /// <returns>
        /// The number of state entries written to the database.
        /// </returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the
        /// database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        /// </returns>
        Task<int> SaveChangesAsync();
        
        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the
        /// database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        /// </returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    }

}