using System.Collections.Generic;
using System.Reflection;
using MongoMigrations.Filters;
using MongoMigrations.Migrations;

namespace MongoMigrations
{
    /// <summary>
    /// Migration Locator
    /// </summary>
    public interface IMigrationLocator
    {
        /// <summary>
        /// Looks the type of for migrations in the assembly that contains type T
        /// </summary>
        /// <typeparam name="T">Type that will serve to look for assemblies</typeparam>
        void LookForMigrationsInAssemblyOfType<T>();

        /// <summary>
        /// Looks for migrations in assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        void LookForMigrationsInAssembly(Assembly assembly);

        /// <summary>
        /// Gets all migrations.
        /// </summary>
        /// <returns>All the migrations</returns>
        IEnumerable<IMigration> GetAllMigrations();

        /// <summary>
        /// Gets the Latest version
        /// </summary>
        /// <returns>Latest version in all migrations</returns>
        MigrationVersion LatestVersion();

        /// <summary>
        /// Gets the migrations after a specific version.
        /// </summary>
        /// <param name="currentVersion">The current version.</param>
        /// <returns>All migrations after specific version</returns>
        IEnumerable<IMigration> GetMigrationsAfter(IAppliedMigration currentVersion);

        /// <summary>
        /// Adds the migration filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        void AddMigrationFilter(IMigrationFilter filter);
    }
}