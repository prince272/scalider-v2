using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scalider.Data.UnitOfWork
{
    
    /// <summary>
    /// Defines the basic functionality of a unit of work.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {

        /// <summary>
        /// Saves all changes made in this unit of work to the data store.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the
        /// data store.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        /// </returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

    }
    
}