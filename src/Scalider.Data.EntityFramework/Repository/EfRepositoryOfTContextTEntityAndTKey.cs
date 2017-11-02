#region # using statements #

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Scalider.Data.Entities;
using Scalider.Data.UnitOfWork;

#endregion

namespace Scalider.Data.Repository
{

    /// <summary>
    /// Provides an implementation of the <see cref="IRepository{TEntity, TKey}"/>
    /// generic interface that uses Entity Framework to read and write on the
    /// database.
    /// </summary>
    /// <typeparam name="TContext">The type encapsulating the database
    /// context.</typeparam>
    /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
    /// <typeparam name="TKey">The type encapsulating the primary key of the
    /// entity.</typeparam>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class EfRepository<TContext, TEntity, TKey> :
        EfRepository<TContext, TEntity>, IRepository<TEntity, TKey>
        where TContext : DbContext
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="EfRepository{TContext, TEntity,TKey}"/> class.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public EfRepository([NotNull] EfUnitOfWork<TContext> unitOfWork)
            : base(unitOfWork)
        {
        }

        #region # IRepository<TEntity,TKey> #

        /// <inheritdoc />
        public virtual TEntity Find(TKey id) =>
            EqualityComparer<TKey>.Default.Equals(id, default(TKey))
                ? default(TEntity)
                : DbSet.Find(id);

        /// <inheritdoc />
        public virtual Task<TEntity> FindAsync(TKey id,
            CancellationToken cancellationToken = new CancellationToken()) =>
            EqualityComparer<TKey>.Default.Equals(id, default(TKey))
                ? Task.FromResult(default(TEntity))
                : DbSet.FindAsync(new object[] {id}, cancellationToken);

        #endregion

    }

}