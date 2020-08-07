// Copyright (c)  Maikebing. All rights reserved.
// Licensed under the MIT License, See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Maikebing.EntityFrameworkCore.Taos.Diagnostics.Internal;
using Maikebing.EntityFrameworkCore.Taos.Infrastructure.Internal;
using Maikebing.EntityFrameworkCore.Taos.Internal;
using Maikebing.EntityFrameworkCore.Taos.Migrations.Internal;
using Maikebing.EntityFrameworkCore.Taos.Query.Internal;
using Maikebing.EntityFrameworkCore.Taos.Storage.Internal;
using Maikebing.EntityFrameworkCore.Taos.Update.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Taos specific extension methods for <see cref="IServiceCollection" />.
    /// </summary>
    public static class TaosServiceCollectionExtensions
    {
        /// <summary>
        ///     <para>
        ///         Adds the services required by the Taos database provider for Entity Framework
        ///         to an <see cref="IServiceCollection" />.
        ///     </para>
        ///     <para>
        ///         Calling this method is no longer necessary when building most applications, including those that
        ///         use dependency injection in ASP.NET or elsewhere.
        ///         It is only needed when building the internal service provider for use with
        ///         the <see cref="DbContextOptionsBuilder.UseInternalServiceProvider" /> method.
        ///         This is not recommend other than for some advanced scenarios.
        ///     </para>
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddEntityFrameworkTaos([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection));

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<LoggingDefinitions, TaosLoggingDefinitions>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<TaosOptionsExtension>>()
                .TryAdd<IRelationalTypeMappingSource, TaosTypeMappingSource>()
                .TryAdd<ISqlGenerationHelper, TaosSqlGenerationHelper>()
                .TryAdd<IMigrationsAnnotationProvider, TaosMigrationsAnnotationProvider>()
                .TryAdd<IModelValidator, TaosModelValidator>()
                .TryAdd<IProviderConventionSetBuilder, TaosConventionSetBuilder>()
                .TryAdd<IUpdateSqlGenerator, TaosUpdateSqlGenerator>()
                .TryAdd<IModificationCommandBatchFactory, TaosModificationCommandBatchFactory>()
                .TryAdd<IRelationalConnection>(p => p.GetService<ITaosRelationalConnection>())
                .TryAdd<IMigrationsSqlGenerator, TaosMigrationsSqlGenerator>()
                .TryAdd<IRelationalDatabaseCreator, TaosDatabaseCreator>()
                .TryAdd<IHistoryRepository, TaosHistoryRepository>()

                // New Query Pipeline
                .TryAdd<IMethodCallTranslatorProvider, TaosMethodCallTranslatorProvider>()
                .TryAdd<IMemberTranslatorProvider, TaosMemberTranslatorProvider>()
                .TryAdd<IQuerySqlGeneratorFactory, TaosQuerySqlGeneratorFactory>()
                .TryAdd<IQueryableMethodTranslatingExpressionVisitorFactory, TaosQueryableMethodTranslatingExpressionVisitorFactory>()
                .TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, TaosSqlTranslatingExpressionVisitorFactory>()
                .TryAddProviderSpecificServices(
                    b => b.TryAddScoped<ITaosRelationalConnection, TaosRelationalConnection>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
