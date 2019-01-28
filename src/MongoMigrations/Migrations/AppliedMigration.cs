using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoMigrations.Migrations
{
    /// <summary>
    /// Represents an applied Migration
    /// </summary>
    public class AppliedMigration : IAppliedMigration
    {
        private const string ManuallyMarked = "Manually marked";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedMigration"/> class.
        /// </summary>
        public AppliedMigration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedMigration"/> class.
        /// </summary>
        /// <param name="migration">The migration.</param>
        public AppliedMigration(IMigration migration)
        {
            Version = migration.Version;
            StartedOn = DateTime.Now;
            Description = migration.Description;
        }

        /// <inheritdoc />
        [BsonId]
        public MigrationVersion Version { get; private set; }

        /// <inheritdoc />
        public string Description { get; private set; }

        /// <inheritdoc />
        public DateTime StartedOn { get; private set; }

        /// <inheritdoc />
        public DateTime? CompletedOn { get; private set; }

        /// <summary>
        /// Creates a manually marked migration
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>Manually marked migration</returns>
        public static AppliedMigration MarkerOnly(MigrationVersion version)
        {
            return new AppliedMigration
            {
                Version = version,
                Description = ManuallyMarked,
                StartedOn = DateTime.Now,
                CompletedOn = DateTime.Now
            };
        }

        /// <inheritdoc />
        public void Complete()
        {
            CompletedOn = DateTime.UtcNow;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Version} started on {StartedOn} completed on {CompletedOn}";
        }
    }
}