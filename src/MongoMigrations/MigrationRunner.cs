using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoMigrations.Migrations;

namespace MongoMigrations
{
    /// <summary>
    /// Migration Runner
    /// </summary>
    public class MigrationRunner : IMigrationRunner
    {
        /// <summary>
        /// The migration target
        /// </summary>
        private readonly IMigrationTarget _migrationTarget;

        /// <summary>
        /// The database
        /// </summary>
        private readonly IMongoDatabase _database;

        /// <summary>
        /// The database status
        /// </summary>
        private readonly IDatabaseMigrationStatus _databaseStatus;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        static MigrationRunner()
        {
            BsonSerializer.RegisterSerializer(typeof(MigrationVersion), new MigrationVersionSerializer());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationRunner" /> class.
        /// </summary>
        /// <param name="database">The database.</param>
        public MigrationRunner(IMongoDatabase database)
        {
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddMongoMigrations()
                .AddLogging(cfg =>
                {
                    cfg.AddConsole();
                });

            serviceCollection.AddSingleton(database);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            _database = database;

            _migrationTarget = serviceProvider.GetService<IMigrationTarget>();
            _databaseStatus = _databaseStatus = serviceProvider.GetService<IDatabaseMigrationStatus>();
            _logger = serviceProvider.GetService<ILogger<MigrationRunner>>();
            MigrationLocator = serviceProvider.GetService<IMigrationLocator>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationRunner" /> class.
        /// </summary>
        /// <param name="migrationTarget">The migration target.</param>
        /// <param name="database">The database.</param>
        /// <param name="migrationLocator">The migration locator.</param>
        /// <param name="databaseStatus">The database status.</param>
        /// <param name="logger">The logger.</param>
        public MigrationRunner(IMigrationTarget migrationTarget, IMongoDatabase database, IMigrationLocator migrationLocator, IDatabaseMigrationStatus databaseStatus, ILogger<MigrationRunner> logger)
        {
            _migrationTarget = migrationTarget ?? throw new ArgumentNullException(nameof(migrationTarget));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _databaseStatus = databaseStatus ?? throw new ArgumentNullException(nameof(databaseStatus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            MigrationLocator = migrationLocator ?? throw new ArgumentNullException(nameof(migrationLocator));
        }

        /// <summary>
        /// Gets the migration locator.
        /// </summary>
        public IMigrationLocator MigrationLocator { get; private set; }

        /// <summary>
        /// Updates the Database to Latest Migration.
        /// </summary>
        public virtual void UpdateToLatest()
        {
            _logger.LogInformation($"{_migrationTarget} to latest...");
            UpdateTo(MigrationLocator.LatestVersion());
        }

        /// <summary>
        /// Updates to a specific version.
        /// </summary>
        /// <param name="updateToVersion">The update to version.</param>
        public virtual void UpdateTo(MigrationVersion updateToVersion)
        {
            var currentVersion = _databaseStatus.GetLastAppliedMigration();
            _logger.LogInformation($"Updating server(s) \"{_migrationTarget.ServerAddresses}\" for database \"{_migrationTarget.DatabaseName}\", from version {currentVersion} to version {updateToVersion}");

            var migrations = MigrationLocator.GetMigrationsAfter(currentVersion)
                .Where(m => m.Version <= updateToVersion);

            ApplyMigrations(migrations);
        }

        /// <summary>
        /// Applies the migrations.
        /// </summary>
        /// <param name="migrations">The migrations.</param>
        protected virtual void ApplyMigrations(IEnumerable<IMigration> migrations)
        {
            if (!migrations.Any())
            {
                _logger.LogInformation($"No migration found");
            }

            migrations.ToList()
                .ForEach(ApplyMigration);
        }

        /// <summary>
        /// Applies a single migration.
        /// </summary>
        /// <param name="migration">The migration.</param>
        protected virtual void ApplyMigration(IMigration migration)
        {
            _logger.LogInformation($"Applying migration version {migration.Version} \"{migration.Description}\" to database {_migrationTarget.DatabaseName}");

            var appliedMigration = _databaseStatus.StartMigration(migration);
            try
            {
                migration.Update(_database);
            }
            catch (Exception exception)
            {
                OnMigrationException(migration, exception);
            }

            _databaseStatus.CompleteMigration(appliedMigration);

            _logger.LogInformation($"Migration version {migration.Version} \"{migration.Description}\" successfully to database {_migrationTarget.DatabaseName}");
        }

        /// <summary>
        /// Called when a migration exception occurs.
        /// </summary>
        /// <param name="migration">The migration.</param>
        /// <param name="exception">The exception.</param>
        protected virtual void OnMigrationException(IMigration migration, Exception exception)
        {
            var message = new
            {
                Message = $"Migration failed to be applied: {exception.Message}",
                migration.Version,
                Name = migration.GetType(),
                migration.Description,
                DatabaseName = _migrationTarget.DatabaseName
            };

            _logger.LogError(exception, $"Migration version {migration.Version} \"{migration.Description}\" from type \"{migration.GetType().FullName}\" to database {_migrationTarget.DatabaseName} failed to be applied");
            throw new MigrationException(message.ToString(), exception);
        }
    }
}