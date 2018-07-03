using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Scalider.Domain.Entity;

namespace Scalider.Domain.Repository
{

    /// <summary>
    /// Implementation of the <see cref="IRepository{TEntity,TKey}"/> interface that uses Entity Framework Core.
    /// </summary>
    /// <typeparam name="TContext">The type encapsulating the database context.</typeparam>
    /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
    /// <typeparam name="TKey">The type encapsulating the primary key of the entity.</typeparam>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class EfRepository<TContext, TEntity, TKey> : EfRepository<TContext, TEntity>, IRepository<TEntity, TKey>
        where TContext : DbContext
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IEquatable<TKey>
    {

        private const double DoubleIdTolerance = 0.05d;
        private const float SingleIdTolerance = 0.05f;

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
        public virtual TEntity FindById(TKey id)
        {
            if (EqualityComparer<TKey>.Default.Equals(id, default))
            {
                // We are not going to try to retrieve the entity when the default value for the type is provided
                return null;
            }
            
            // Try to find the entity with a simple primary key type
            var baseQuery = DbSet.AsNoTracking();
            switch (id)
            {
                case int intId:
                    return baseQuery.FirstOrDefault(t => EF.Property<int>(t, "Id") == intId);
                case decimal decimalId:
                    return baseQuery.FirstOrDefault(t => EF.Property<decimal>(t, "Id") == decimalId);
                case double doubleId:
                    return baseQuery
                        .FirstOrDefault(t => Math.Abs(EF.Property<double>(t, "Id") - doubleId) < DoubleIdTolerance);
                case float floatId:
                    return baseQuery
                        .FirstOrDefault(t => Math.Abs(EF.Property<float>(t, "Id") - floatId) < SingleIdTolerance);
                case Guid guid:
                    return baseQuery.FirstOrDefault(t => EF.Property<Guid>(t, "Id") == guid);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <inheritdoc />
        public virtual Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            if (EqualityComparer<TKey>.Default.Equals(id, default))
            {
                // We are not going to try to retrieve the entity when the default value for the type is provided
                return Task.FromResult<TEntity>(null);
            }
            
            // Try to find the entity with a simple primary key type
            var baseQuery = DbSet.AsNoTracking();
            switch (id)
            {
                case int intId:
                    return baseQuery
                        .FirstOrDefaultAsync(t => EF.Property<int>(t, "Id") == intId, cancellationToken);
                case decimal decimalId:
                    return baseQuery
                        .FirstOrDefaultAsync(t => EF.Property<decimal>(t, "Id") == decimalId, cancellationToken);
                case double doubleId:
                    return baseQuery
                        .FirstOrDefaultAsync(
                            t => Math.Abs(EF.Property<double>(t, "Id") - doubleId) < DoubleIdTolerance,
                            cancellationToken
                        );
                case float floatId:
                    return baseQuery
                        .FirstOrDefaultAsync(
                            t => Math.Abs(EF.Property<float>(t, "Id") - floatId) < SingleIdTolerance,
                            cancellationToken
                        );
                case Guid guid:
                    return baseQuery
                        .FirstOrDefaultAsync(t => EF.Property<Guid>(t, "Id") == guid, cancellationToken);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

    }

}