using MongoMigrations.Migrations;

namespace MongoMigrations.Filters
{
    /// <summary>
    /// Represents a Migration Filter
    /// </summary>
    public interface IMigrationFilter
    {
        /// <summary>
        /// Excludes the specified migration.
        /// </summary>
        /// <param name="migration">The migration.</param>
        /// <returns>True if the migration should be excluded. False otherwise</returns>
        bool Exclude(IMigration migration);
    }
}