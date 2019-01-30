using System;
using Microsoft.Extensions.DependencyInjection;
using MongoMigrations.Contiguration;

namespace MongoMigrations
{
    /// <summary>
    /// Extension Methods for ServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the mongo migrations to the service collection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configurationDelegate">The configuration delegate.</param>
        public static IServiceCollection AddMongoMigrations(this IServiceCollection services, Action<MongoMigrationConfiguration> configurationDelegate = null)
        {
            var configuration = new MongoMigrationConfiguration();
            configurationDelegate?.Invoke(configuration);

            services.AddSingleton(configuration);

            services.AddTransient<IMigrationTarget, MigrationTarget>();
            services.AddTransient<IMigrationRunner, MigrationRunner>();
            services.AddTransient<IMigrationLocator, MigrationLocator>();
            services.AddTransient<IDatabaseMigrationStatus, DatabaseMigrationStatus>();
            return services;
        }
    }
}