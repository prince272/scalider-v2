#region # using statements #

using System;

#endregion

namespace Scalider.Data.UnitOfWork
{

    /// <summary>
    /// A transaction against the database.
    /// </summary>
    public interface IUnitOfWorkTransaction : IDisposable
    {

        /// <summary>
        /// Commits all changes made to the database in the current transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Discards all changes made to the database in the current transaction.
        /// </summary>
        void Rollback();

    }

}