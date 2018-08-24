using Microsoft.EntityFrameworkCore.Storage;

namespace Scalider.Domain.UnitOfWork
{

    internal class EfUnitOfWorkTransaction : IUnitOfWorkTransaction
    {

        private readonly IDbContextTransaction _innerTransaction;

        public EfUnitOfWorkTransaction(IDbContextTransaction transaction)
        {
            _innerTransaction = transaction;
        }

        #region IUnitOfWorkTransaction Members

        /// <inheritdoc />
        public string Id => _innerTransaction.TransactionId.ToString();

        /// <inheritdoc />
        public void Dispose() => _innerTransaction.Dispose();

        /// <inheritdoc />
        public void Commit() => _innerTransaction.Commit();

        /// <inheritdoc />
        public void Rollback() => _innerTransaction.Rollback();

        #endregion

    }

}