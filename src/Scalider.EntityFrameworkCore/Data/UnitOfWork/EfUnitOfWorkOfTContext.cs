using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Scalider.Data.UnitOfWork
{

    /// <summary>
    /// Provides an implementation of the <see cref="IUnitOfWork"/> interface that uses Entity Framework Core.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class EfUnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {

        private readonly TContext _dbContext;

        //        private readonly IServiceScope _serviceScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="EfUnitOfWork{TContext}"/> class.
        /// </summary>
        /// <param name="dbContext"></param>
        public EfUnitOfWork([NotNull] TContext dbContext)
        {
            Check.NotNull(dbContext, nameof(dbContext));

            _dbContext = dbContext;
        }

        #region IUnitOfWork Members

        /// <inheritdoc />
        public virtual void SaveChanges() => _dbContext.SaveChanges();

        //        {
        //            bool saveFailed;
        //
        //            do
        //            {
        //                saveFailed = false;
        //
        //                try
        //                {
        //                    _dbContext.SaveChanges();
        //                }
        //                catch (DbUpdateConcurrencyException ex)
        //                {
        //                    saveFailed = true;
        //                    if (!TryResolveConcurrencyException(ex.Entries))
        //                    {
        //                        // We couldn't solve the concurrency exception
        //                        throw;
        //                    }
        //                }
        //            } while (saveFailed);
        //        }

        /// <inheritdoc />
        public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _dbContext.SaveChangesAsync(cancellationToken);

        //        {
        //            bool saveFailed;
        //
        //            do
        //            {
        //                saveFailed = false;
        //
        //                try
        //                {
        //                    await _dbContext.SaveChangesAsync(cancellationToken);
        //                }
        //                catch (DbUpdateConcurrencyException ex)
        //                {
        //                    saveFailed = true;
        //                    if (!await TryResolveConcurrencyExceptionAsync(ex.Entries, cancellationToken))
        //                    {
        //                        // We couldn't solve the concurrency exception
        //                        throw;
        //                    }
        //                }
        //            } while (saveFailed);
        //        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            // no-op
        }

        #endregion

        //        private bool TryResolveConcurrencyException(IEnumerable<EntityEntry> entries)
        //        {
        //            foreach (var entry in entries)
        //            {
        //                // Get the current entity values and the values in the database 
        //                // as instances of the entity type 
        //                var databaseValues = entry?.GetDatabaseValues();
        //                if (databaseValues == null)
        //                    return false;
        //                
        //                var databaseValuesAsObject = databaseValues.ToObject();
        //                
        //                // Choose an initial set of resolved values. In this case we 
        //                // make the default be the values currently in the database
        //                var resolvedValuesAsObject = databaseValues.ToObject();
        //                
        //                // Try to resolve concurrency for the entity
        //                if (!TryResolveEntity(entry, entry.Entity, databaseValuesAsObject,
        //                    resolvedValuesAsObject))
        //                {
        //                    // Could not resolve concurrency for the entity
        //                    return false;
        //                }
        //
        //                // Update the original values with the database values and 
        //                // the current values with whatever the user choose
        //                entry.OriginalValues?.SetValues(databaseValues); 
        //                entry.CurrentValues?.SetValues(resolvedValuesAsObject); 
        //            }
        //            
        //            // We got here, assume all conflicts where resolved
        //            return true;
        //        }
        //
        //
        //        private async Task<bool> TryResolveConcurrencyExceptionAsync(IEnumerable<EntityEntry> entries,
        //            CancellationToken cancellationToken)
        //        {
        //            foreach (var entry in entries)
        //            {
        //                if (entry == null)
        //                    return false;
        //
        //                // Get the current entity values and the values in the database 
        //                // as instances of the entity type 
        //                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
        //                if (databaseValues == null)
        //                    return false;
        //
        //                var databaseValuesAsObject = databaseValues.ToObject();
        //
        //                // Choose an initial set of resolved values. In this case we 
        //                // make the default be the values currently in the database
        //                var resolvedValuesAsObject = databaseValues.ToObject();
        //
        //                // Try to resolve concurrency for the entity
        //                if (!TryResolveEntity(entry, entry.Entity, databaseValuesAsObject, resolvedValuesAsObject))
        //                {
        //                    // Could not resolve concurrency for the entity
        //                    return false;
        //                }
        //
        //                // Update the original values with the database values and 
        //                // the current values with whatever the user choose
        //                entry.OriginalValues?.SetValues(databaseValues);
        //                entry.CurrentValues?.SetValues(resolvedValuesAsObject);
        //            }
        //
        //            // We got here, assume all conflicts where resolved
        //            return true;
        //        }
        //
        //        private bool TryResolveEntity(EntityEntry entityEntry, object entity, object databaseEntity,
        //            object resolvedEntity)
        //        {
        //            var entityType = entityEntry.Metadata.ClrType;
        //            var resolverType = typeof(IConcurrencyResolver<>).MakeGenericType(entityType);
        //
        //            // Retrieve the resolver
        //            var resolver = _serviceScope.ServiceProvider.GetService(resolverType);
        //            if (resolver == null)
        //            {
        //                // No resolver available for the current entity
        //                return false;
        //            }
        //
        //            // Execute the resolver
        //            resolver.GetType()
        //                    .GetTypeInfo()
        //                    .DeclaredMethods
        //                    .Single(t => t.Name == "Resolve")
        //                    .MakeGenericMethod(entityType)
        //                    .Invoke(resolver, new[] {entityEntry, entity, databaseEntity, resolvedEntity});
        //
        //            // Done
        //            return true;
        //        }

    }

}