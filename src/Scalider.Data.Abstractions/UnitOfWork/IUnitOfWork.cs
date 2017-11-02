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
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous transaction initialization.
        /// </returns>
        Task<IUnitOfWorkTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = new CancellationToken());

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
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        /// </returns>
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = new CancellationToken());

    }

}