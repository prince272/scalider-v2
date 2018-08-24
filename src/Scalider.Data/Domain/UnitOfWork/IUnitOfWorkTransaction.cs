using System;

namespace Scalider.Domain.UnitOfWork
{

    /// <summary>
    /// A transaction against the database.
    /// </summary>
    public interface IUnitOfWorkTransaction : IDisposable
    {

        /// <summary>
        /// Gets a value indicating the transaction identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Commits all the changes made to the database in the current transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Discards all the changes made to the database in the current transaction.
        /// </summary>
        void Rollback();

    }

}