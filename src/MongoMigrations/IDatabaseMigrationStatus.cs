using System.Collections.Generic;
using MongoDB.Driver;
using MongoMigrations.Migrations;

namespace MongoMigrations
{
    /// <summary>
    /// Database Migration Status
    /// </summary>
    public interface IDatabaseMigrationStatus
    {
        /// <summary>
        /// Gets the applied migrations collection.
        /// </summary>
        /// <returns>Migrations that are applied</returns>
        IMongoCollection<AppliedMigration> GetAppliedMigrationsCollection();

        /// <summary>
        /// Determines whether the database is at latest version or not.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the database is at latest version; otherwise, <c>false</c>.
        /// </returns>
        bool IsNotLatestVersion();

        /// <summary>
        /// Throws an exception if database is not at latest version.
        /// </summary>
        void ThrowIfNotLatestVersion();

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <returns>Version of the database</returns>
        MigrationVersion GetVersion();

        /// <summary>
        /// Gets the last applied migration.
        /// </summary>
        /// <returns>Last applied migration</returns>
        IAppliedMigration GetLastAppliedMigration();

        /// <summary>
        /// Starts the migration.
        /// </summary>
        /// <param name="migration">The migration.</param>
        /// <returns>started migration</returns>
        IAppliedMigration StartMigration(IMigration migration);

        /// <summary>
        /// Completes the migration.
        /// </summary>
        /// <param name="appliedMigration">The applied migration.</param>
        void CompleteMigration(IAppliedMigration appliedMigration);

        /// <summary>
        /// Marks up to version.
        /// </summary>
        /// <param name="version">The version.</param>
        void MarkUpToVersion(MigrationVersion version);

        /// <summary>
        /// Marks the version.
        /// </summary>
        /// <param name="version">The version.</param>
        void MarkVersion(MigrationVersion version);
    }
}