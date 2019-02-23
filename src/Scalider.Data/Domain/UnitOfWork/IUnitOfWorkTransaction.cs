using System;
using JetBrains.Annotations;

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
        [UsedImplicitly]
        string Id { get; }

        /// <summary>
        /// Commits all the changes made to the database in the current transaction.
        /// </summary>
        [UsedImplicitly]
        void Commit();

        /// <summary>
        /// Discards all the changes made to the database in the current transaction.
        /// </summary>
        [UsedImplicitly]
        void Rollback();

    }

}