using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Scalider.Domain.UnitOfWork
{

    /// <summary>
    /// Provides extension methods for the <see cref="IUnitOfWork"/> interface.
    /// </summary>
    public static class UnitOfWorkExtensions
    {

        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/>.</param>
        /// <returns>
        /// A <see cref="IDbContextTransaction" /> that represents the started transaction.
        /// </returns>
        public static IDbContextTransaction BeginTransaction([NotNull] IUnitOfWork unitOfWork)
        {
            Check.NotNull(unitOfWork, nameof(unitOfWork));
            return GetEfUnitOfWorkOrThrow(unitOfWork)
                   .DbContext
                   .Database
                   .BeginTransaction();
        }

        /// <summary>
        /// Asynchronously starts a new transaction.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous transaction initialization. The task result contains
        /// a <see cref="IDbContextTransaction" /> that represents the started transaction.
        /// </returns>
        public static Task<IDbContextTransaction> BeginTransactionAsync([NotNull] IUnitOfWork unitOfWork,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(unitOfWork, nameof(unitOfWork));
            return GetEfUnitOfWorkOrThrow(unitOfWork)
                   .DbContext
                   .Database
                   .BeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Sets the <see cref="DbTransaction" /> to be used by database operations on the
        /// <see cref="DbContext" />.
        /// </summary>
        /// <param name="unitOfWork">The <see cref="IUnitOfWork"/>.</param>
        /// <param name="dbContextTransaction">The <see cref="IDbContextTransaction" /> to use.</param>
        /// <returns>
        /// A <see cref="IDbContextTransaction" /> that encapsulates the given transaction.
        /// </returns>
        public static IDbContextTransaction UseTransaction([NotNull] IUnitOfWork unitOfWork,
            [NotNull] IDbContextTransaction dbContextTransaction)
        {
            Check.NotNull(unitOfWork, nameof(unitOfWork));
            Check.NotNull(dbContextTransaction, nameof(dbContextTransaction));

            return GetEfUnitOfWorkOrThrow(unitOfWork)
                   .DbContext
                   .Database
                   .UseTransaction(dbContextTransaction.GetDbTransaction());
        }

        private static IEfUnitOfWork GetEfUnitOfWorkOrThrow(IUnitOfWork unitOfWork)
        {
            if (unitOfWork is IEfUnitOfWork efUnitOfWork)
                return efUnitOfWork;

            // The type of the sender doesn't support MailKit
            var typeName = nameof(IEfUnitOfWork);
            throw new ArgumentException(
                $"The email sender must be an instance of the {typeName} class",
                nameof(unitOfWork)
            );
        }

    }

}