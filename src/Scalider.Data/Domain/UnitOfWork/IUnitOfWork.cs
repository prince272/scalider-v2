using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Scalider.Domain.UnitOfWork
{

    /// <summary>
    /// Defines the basic functionality of a unit of work.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {

        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        [UsedImplicitly]
        void SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        /// </returns>
        [UsedImplicitly]
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        /// <returns>
        /// A <see cref="IUnitOfWorkTransaction" /> that represents the started transaction.
        /// </returns>
        [UsedImplicitly]
        IUnitOfWorkTransaction BeginTransaction();

        /// <summary>
        /// Asynchronously starts a new transaction.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous transaction initialization. The task result contains
        /// a <see cref="IUnitOfWorkTransaction" /> that represents the started transaction.
        /// </returns>
        [UsedImplicitly]
        Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    }

}