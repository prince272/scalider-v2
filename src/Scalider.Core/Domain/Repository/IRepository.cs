﻿using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Scalider.Domain.Repository
{

    /// <summary>
    /// A repository represents a session with the data store that can be used to retrieve, update and
    /// delete entities. 
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IRepository
    {

        /// <summary>
        /// Returns the total number of entities for this repository.
        /// </summary>
        /// <returns>
        /// The total number of entities for this repository.
        /// </returns>
        int Count();

        /// <summary>
        /// Asynchronously returns the total number of entities for this repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the
        /// task to complete.</param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

    }

}