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

        public string Id => _innerTransaction.TransactionId.ToString();

        public void Dispose() => _innerTransaction.Dispose();

        public void Commit() => _innerTransaction.Commit();

        public void Rollback() => _innerTransaction.Rollback();

    }

}