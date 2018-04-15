using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Scalider.Data.Entity;

namespace Scalider.Data.Repository
{

    /// <summary>
    /// Provides an implementation of the <see cref="IRepository{TEntity, TKey}"/> generic interface that uses Entity
    /// Framework Core to read and write on the database.
    /// </summary>
    /// <typeparam name="TContext">The type encapsulating the database context.</typeparam>
    /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
    /// <typeparam name="TKey">The type encapsulating the primary key of the entity.</typeparam>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global"),
     SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class EfRepository<TContext, TEntity, TKey> : EfRepository<TContext, TEntity>, IRepository<TEntity, TKey>
        where TContext : DbContext
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepository{TContext, TEntity,TKey}"/> class.
        /// </summary>
        /// <param name="context"></param>
        [UsedImplicitly]
        public EfRepository([NotNull] TContext context)
            : base(context)
        {
        }

        #region IRepository<TEntity,TKey> Members

        /// <inheritdoc />
        public virtual TEntity FindById(TKey id) => EqualityComparer<TKey>.Default.Equals(id, default)
            ? default
            : GetQueryableWithIncludes().FirstOrDefault(t => EqualityComparer<TKey>.Default.Equals(t.Id, id));

        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default) =>
            EqualityComparer<TKey>.Default.Equals(id, default)
                ? Task.FromResult(default(TEntity))
                : GetQueryableWithIncludes().FirstOrDefaultAsync(t => EqualityComparer<TKey>.Default.Equals(t.Id, id),
                    cancellationToken);

        #endregion

    }

}