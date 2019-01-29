using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoMigrations.Migrations;
using Moq;
using Xunit;

namespace MongoMigrations.Tests
{
    /// <summary>
    /// Unit Tests for Migration Runner
    /// </summary>
    public class MigrationRunnerTests : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// UpdateToLatest should call all pending migrations
        /// </summary>
        [Fact]
        public void UpdateToLatestShouldApplyAllPendingMigrations()
        {
            _context
                .SetupMigrationTarget()
                .SetupUpgradeToLatest()
                .SetupLastAppliedMigration()
                .SetupMigrationsToApply();

            _context.Sut.UpdateToLatest();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _context.Dispose();
            }
        }

        /// <summary>
        /// The test Context
        /// </summary>
        private class TestContext : IDisposable
        {
            /// <summary>
            /// The random generator
            /// </summary>
            private readonly Random _randomGenerator = new Random();

            /// <summary>
            /// The database
            /// </summary>
            private readonly Mock<IMongoDatabase> _database = new Mock<IMongoDatabase>(MockBehavior.Strict);

            /// <summary>
            /// The migration target
            /// </summary>
            private readonly Mock<IMigrationTarget> _migrationTarget = new Mock<IMigrationTarget>(MockBehavior.Strict);

            /// <summary>
            /// The migration locator
            /// </summary>
            private readonly Mock<IMigrationLocator> _migrationLocator = new Mock<IMigrationLocator>(MockBehavior.Strict);

            /// <summary>
            /// The migration status
            /// </summary>
            private readonly Mock<IDatabaseMigrationStatus> _migrationStatus = new Mock<IDatabaseMigrationStatus>(MockBehavior.Strict);

            /// <summary>
            /// The logger
            /// </summary>
            private readonly Mock<ILogger<MigrationRunner>> _logger = new Mock<ILogger<MigrationRunner>>();

            /// <summary>
            /// The applied migration
            /// </summary>
            private Mock<IAppliedMigration> _appliedMigration;

            private string _serverAddresses;

            private string _databaseName;

            private MigrationVersion _latestVersion;

            private string _migrationTargetDescription;

            private string _appliedMigrationDescription;

            private Mock<IMigration>[] _migrationsToApply;

            private IList<Mock<IAppliedMigration>> _newAppliedMigrations = new List<Mock<IAppliedMigration>>();

            /// <summary>
            /// Initializes a new instance of the <see cref="TestContext"/> class.
            /// </summary>
            public TestContext()
            {
                Sut = new MigrationRunner(
                    _migrationTarget.Object,
                    _database.Object,
                    _migrationLocator.Object,
                    _migrationStatus.Object,
                    _logger.Object);
            }

            /// <summary>
            /// Gets the system under test
            /// </summary>
            public IMigrationRunner Sut { get; }

            public TestContext SetupMigrationTarget()
            {
                _serverAddresses = Guid.NewGuid().ToString();
                _databaseName = Guid.NewGuid().ToString();
                _latestVersion = GenerateRandomVersion();
                _migrationTargetDescription = Guid.NewGuid().ToString();
                _migrationTarget.Setup(x => x.ToString()).Returns(_migrationTargetDescription);
                _migrationTarget.SetupGet(x => x.ServerAddresses).Returns(_serverAddresses);
                _migrationTarget.SetupGet(x => x.DatabaseName).Returns(_databaseName);

                return this;
            }

            public TestContext SetupUpgradeToLatest()
            {
                _migrationLocator.Setup(x => x.LatestVersion()).Returns(_latestVersion);
                return this;
            }

            public TestContext SetupLastAppliedMigration()
            {
                _appliedMigrationDescription = Guid.NewGuid().ToString();
                _appliedMigration = new Mock<IAppliedMigration>(MockBehavior.Strict);
                _appliedMigration.Setup(x => x.ToString()).Returns(_appliedMigrationDescription);
                _migrationStatus.Setup(x => x.GetLastAppliedMigration()).Returns(_appliedMigration.Object);
                return this;
            }

            public TestContext SetupMigrationsToApply()
            {
                _migrationsToApply = Enumerable.Range(1, 5).Select(i => new Mock<IMigration>(MockBehavior.Strict)).ToArray();
                _migrationLocator.Setup(x => x.GetMigrationsAfter(_appliedMigration.Object))
                    .Returns(_migrationsToApply.Select(x => x.Object));

                SetupSingleMigration();

                return this;
            }

            /// <inheritdoc />
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
            protected virtual void Dispose(bool isDisposing)
            {
                if (isDisposing)
                {
                    _database.VerifyAll();
                    _migrationTarget.VerifyAll();
                    _migrationLocator.VerifyAll();
                    _migrationStatus.VerifyAll();
                    _logger.VerifyAll();

                    _appliedMigration?.VerifyAll();
                    foreach (var migration in _migrationsToApply)
                    {
                        migration.VerifyAll();
                    }

                    foreach (var newAppliedMigration in _newAppliedMigrations)
                    {
                        newAppliedMigration.VerifyAll();
                    }
                }
            }

            private void SetupSingleMigration()
            {
                foreach (var migration in _migrationsToApply.Take(3))
                {
                    var description = Guid.NewGuid().ToString();
                    var version = GenerateRandomVersionLowerThan(_latestVersion);
                    migration.SetupGet(x => x.Version).Returns(version);
                    migration.SetupGet(x => x.Description).Returns(description);

                    var newAppliedMigration = new Mock<IAppliedMigration>(MockBehavior.Strict);

                    _migrationStatus.Setup(x => x.StartMigration(migration.Object))
                        .Returns(newAppliedMigration.Object);

                    _newAppliedMigrations.Add(newAppliedMigration);

                    migration.Setup(x => x.Update(_database.Object));

                    _migrationStatus.Setup(x => x.CompleteMigration(newAppliedMigration.Object));
                }

                foreach (var migration in _migrationsToApply.Skip(3))
                {
                    var version = GenerateRandomVersionGreaterThan(_latestVersion);
                    migration.SetupGet(x => x.Version).Returns(version);
                }
            }

            private MigrationVersion GenerateRandomVersion()
            {
                return new MigrationVersion(_randomGenerator.Next(10, 20), _randomGenerator.Next(0, 5), _randomGenerator.Next(0, 5));
            }

            private MigrationVersion GenerateRandomVersionLowerThan(MigrationVersion migrationVersion)
            {
                return new MigrationVersion(_randomGenerator.Next(0, migrationVersion.Major - 1), _randomGenerator.Next(0, 5), _randomGenerator.Next(0, 5));
            }

            private MigrationVersion GenerateRandomVersionGreaterThan(MigrationVersion migrationVersion)
            {
                return new MigrationVersion(_randomGenerator.Next(migrationVersion.Major + 1, int.MaxValue), _randomGenerator.Next(0, 5), _randomGenerator.Next(0, 5));
            }
        }
    }
}
