﻿#region # using statements #

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Scalider.Data.Repository
{

    /// <summary>
    /// A repository represents a session with the database that can be used to
    /// retrieve, update and delete entities. 
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IRepository
    {

        /// <summary>
        /// Gets the count of all entities in this repository.
        /// </summary>
        /// <returns>
        /// The number of entities in this repository.
        /// </returns>
        int Count();

        /// <summary>
        /// Asynchronously gets the count of all entities in this repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        Task<int> CountAsync(
            CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Gets the count of all entities in this repository.
        /// </summary>
        /// <returns>
        /// The number of entities in this repository.
        /// </returns>
        /// <remarks>
        /// Use this method when the return value is expected to be greater
        /// than <see cref="int.MaxValue"/>.
        /// </remarks>
        long LongCount();

        /// <summary>
        /// Asynchronously gets the count of all entities in this repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to
        /// observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous
        /// operation.
        /// </returns>
        /// <remarks>
        /// Use this method when the return value is expected to be greater
        /// than <see cref="int.MaxValue"/>.
        /// </remarks>
        Task<long> LongCountAsync(
            CancellationToken cancellationToken = new CancellationToken());

    }

}