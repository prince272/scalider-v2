using Microsoft.EntityFrameworkCore;

namespace Scalider.Domain.UnitOfWork
{
    
    /// <summary>
    /// Defines the basic functionality of a unit of work that uses Entity Framework Core to operate against.
    /// </summary>
    public interface IEfUnitOfWork : IUnitOfWork
    {
        
        /// <summary>
        /// Gets the <see cref="Microsoft.EntityFrameworkCore.DbContext"/> for this unit of work.
        /// </summary>
        DbContext DbContext { get; }
        
    }
    
}