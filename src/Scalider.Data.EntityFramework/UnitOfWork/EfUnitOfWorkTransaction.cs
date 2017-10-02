#region # using statements #

using Microsoft.EntityFrameworkCore.Storage;

#endregion

namespace Scalider.Data.UnitOfWork
{
    
    internal class EfUnitOfWorkTransaction : IUnitOfWorkTransaction
    {

        #region # Variables #

        private readonly IDbContextTransaction _transaction;

        #endregion

        public EfUnitOfWorkTransaction(IDbContextTransaction transaction)
        {
            Check.NotNull(transaction, nameof(transaction));
            _transaction = transaction;
        }

        #region # IDisposable #

        public void Dispose() => _transaction.Dispose();

        #endregion

        #region # IUnitOfWorkTransaction #

        public void Commit() => _transaction.Commit();

        public void Rollback() => _transaction.Rollback();

        #endregion

    }
    
}