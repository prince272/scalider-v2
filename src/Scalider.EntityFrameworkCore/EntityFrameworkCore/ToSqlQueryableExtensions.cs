using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Parsing.Structure;
using Scalider.Data.Entity;

namespace Scalider.EntityFrameworkCore
{
    
    /// <summary>
    /// Provides extension methods for the <see cref="IQueryable{T}"/> interface.
    /// </summary>
    public static class ToSqlQueryableExtensions
    {

        private static readonly Lazy<TypeInfo> QueryCompilerTypeInfo =
            new Lazy<TypeInfo>(() => typeof(QueryCompiler).GetTypeInfo());
        
        private static readonly Lazy<FieldInfo> QueryCompilerField =
            new Lazy<FieldInfo>(() =>
                typeof(EntityQueryProvider)
                    .GetTypeInfo().DeclaredFields
                    .First(x => x.Name == "_queryCompiler"));
        
        private static readonly Lazy<PropertyInfo> NodeTypeProviderField =
            new Lazy<PropertyInfo>(() =>
                QueryCompilerTypeInfo.Value.DeclaredProperties.Single(x =>
                    x.Name == "NodeTypeProvider"));

        private static readonly Lazy<MethodInfo> CreateQueryParserMethod =
            new Lazy<MethodInfo>(() =>
                QueryCompilerTypeInfo.Value.DeclaredMethods.First(x =>
                    x.Name == "CreateQueryParser"));

        private static readonly Lazy<FieldInfo> DataBaseField =
            new Lazy<FieldInfo>(() =>
                QueryCompilerTypeInfo.Value.DeclaredFields.Single(x =>
                    x.Name == "_database"));
        
#if NETSTANDARD2_0
        private static readonly Lazy<PropertyInfo> DatabaseDependenciesField =
            new Lazy<PropertyInfo>(() =>
                typeof(Database).GetTypeInfo().DeclaredProperties
                                .Single(x => x.Name == "Dependencies"));
#else
        private static readonly Lazy<FieldInfo> QueryCompilationContextFactoryField =
            new Lazy<FieldInfo>(() =>
                typeof(Database).GetTypeInfo().DeclaredFields.First(x =>
                    x.Name == "_queryCompilationContextFactory"));
#endif

        /// <summary>
        /// Parses a Linq2Sql query to a string.
        /// </summary>
        /// <param name="query">The query to be parsed.</param>
        /// <typeparam name="TEntity">The type encapsulating the entity.</typeparam>
        /// <returns>
        /// The parsed query as a string.
        /// </returns>
        /// <exception cref="ArgumentException">When the query is not a valid
        /// Entity Framework Core query.</exception>
        public static string ToSql<TEntity>([NotNull] this IQueryable<TEntity> query)
            where TEntity : class, IEntity
        {
            Check.NotNull(query, nameof(query));
            if (!(query is EntityQueryable<TEntity>) &&
                !(query is InternalDbSet<TEntity>))
            {
                // The type of the query doesn't seem to be an Entity Framework Core
                // query
                throw new ArgumentException(
                    "The type of the query is not a valid Entity Framework Core query.",
                    nameof(query)
                );
            }
            
            // Retrieve the query compiler
            var queryCompiler =
                (IQueryCompiler)QueryCompilerField.Value.GetValue(query.Provider);
            
            // Parse the query
            var queryParser = GetQueryParser(queryCompiler);
            var queryModel = queryParser.GetParsedQuery(query.Expression);
            var database = DataBaseField.Value.GetValue(queryCompiler);
            
            // Retrieve the model visitor and create the executor
            var queryCompilationContextFactory =
                GetQueryCompilationContextFactory(database);
            
            var queryCompilationContext =
                queryCompilationContextFactory.Create(false);
            
            var modelVisitor =
                (RelationalQueryModelVisitor)queryCompilationContext
                    .CreateQueryModelVisitor();
            
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);

            // Done
            return modelVisitor.Queries.First().ToString();
        }

        private static IQueryParser GetQueryParser(IQueryCompiler queryCompiler)
        {
            return (IQueryParser)CreateQueryParserMethod.Value.Invoke(
                queryCompiler,
                new[]
                {
                    NodeTypeProviderField.Value.GetValue(queryCompiler)
                }
            );
        }

        private static IQueryCompilationContextFactory
            GetQueryCompilationContextFactory(object database)
        {
#if NETSTANDARD2_0
            var databaseDependencies =
                (DatabaseDependencies)
                DatabaseDependenciesField.Value.GetValue(database);

            return databaseDependencies.QueryCompilationContextFactory;
#else
            return (IQueryCompilationContextFactory)
                QueryCompilationContextFactoryField.Value.GetValue(database);
#endif
        }

    }
}