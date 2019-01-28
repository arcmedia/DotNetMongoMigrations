using System;

namespace MongoMigrations.Migrations
{
    /// <summary>
    /// Migration that has been applied to database
    /// </summary>
    public interface IAppliedMigration
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        MigrationVersion Version { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the started on.
        /// </summary>
        DateTime StartedOn { get; }

        /// <summary>
        /// Gets when the migration has been completed.
        /// </summary>
        DateTime? CompletedOn { get; }

        /// <summary>
        /// Completes the migration.
        /// </summary>
        void Complete();
    }
}