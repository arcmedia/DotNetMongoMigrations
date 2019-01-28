using MongoMigrations.Migrations;

namespace MongoMigrations.Filters
{
    /// <inheritdoc />
    public abstract class MigrationFilter : IMigrationFilter
    {
        /// <inheritdoc />
        public abstract bool Exclude(IMigration migration);
    }
}