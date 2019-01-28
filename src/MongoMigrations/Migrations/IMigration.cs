using MongoDB.Driver;

namespace MongoMigrations.Migrations
{
    /// <summary>
    /// Migration
    /// </summary>
    public interface IMigration
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
        /// Updates this instance.
        /// </summary>
        /// <param name="database">The database.</param>
        void Update(IMongoDatabase database);
    }
}