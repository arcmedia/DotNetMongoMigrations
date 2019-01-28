using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoMigrations.Migrations;

namespace MongoMigrations
{
    /// <inheritdoc />
    public class DatabaseMigrationStatus : IDatabaseMigrationStatus
    {
        private const string VersionCollectionName = "DatabaseVersion";
        private readonly IMongoDatabase _database;
        private readonly IMigrationLocator _migrationLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseMigrationStatus" /> class.
        /// </summary>
        /// <param name="mongoDatabase">The mongo database.</param>
        /// <param name="migrationLocator">The migration locator.</param>
        public DatabaseMigrationStatus(IMongoDatabase mongoDatabase, IMigrationLocator migrationLocator)
        {
            _database = mongoDatabase ?? throw new ArgumentNullException(nameof(mongoDatabase));
            _migrationLocator = migrationLocator ?? throw new ArgumentNullException(nameof(migrationLocator));
        }

        /// <inheritdoc />
        public virtual IMongoCollection<AppliedMigration> GetAppliedMigrationsCollection()
        {
            return _database
                .GetCollection<AppliedMigration>(VersionCollectionName);
        }

        /// <inheritdoc />
        public virtual bool IsNotLatestVersion()
        {
            return _migrationLocator.LatestVersion()
                   != GetVersion();
        }

        /// <inheritdoc />
        public virtual void ThrowIfNotLatestVersion()
        {
            if (!IsNotLatestVersion())
            {
                return;
            }

            var databaseVersion = GetVersion();
            var migrationVersion = _migrationLocator.LatestVersion();
            throw new MigrationException(
                $"Database is not the expected version, database is at version: {databaseVersion}, migrations are at version: {migrationVersion}",
                null);
        }

        /// <inheritdoc />
        public virtual MigrationVersion GetVersion()
        {
            var lastAppliedMigration = GetLastAppliedMigration();
            return lastAppliedMigration == null
                ? MigrationVersion.Default()
                : lastAppliedMigration.Version;
        }

        /// <inheritdoc />
        public virtual IAppliedMigration GetLastAppliedMigration()
        {
            return GetAppliedMigrationsCollection()
                .Find(m => true)
                .ToList() // in memory but this will never get big enough to matter
                .OrderByDescending(v => v.Version)
                .FirstOrDefault();
        }

        /// <inheritdoc />
        public virtual IAppliedMigration StartMigration(IMigration migration)
        {
            var appliedMigration = new AppliedMigration(migration);
            GetAppliedMigrationsCollection()
                .InsertOne(appliedMigration);
            return appliedMigration;
        }

        /// <inheritdoc />
        public virtual void CompleteMigration(IAppliedMigration appliedMigration)
        {
            appliedMigration.Complete();
            GetAppliedMigrationsCollection()
                .ReplaceOne(m => m.Version == appliedMigration.Version, appliedMigration as AppliedMigration);
        }

        /// <inheritdoc />
        public virtual void MarkUpToVersion(MigrationVersion version)
        {
            _migrationLocator.GetAllMigrations()
                .Where(m => m.Version <= version)
                .ToList()
                .ForEach(m => MarkVersion(m.Version));
        }

        /// <inheritdoc />
        public virtual void MarkVersion(MigrationVersion version)
        {
            var appliedMigration = AppliedMigration.MarkerOnly(version);
            GetAppliedMigrationsCollection()
                .InsertOne(appliedMigration);
        }
    }
}