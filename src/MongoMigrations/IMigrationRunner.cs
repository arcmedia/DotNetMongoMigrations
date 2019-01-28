namespace MongoMigrations
{
    /// <summary>
    /// Migration Runner
    /// </summary>
    public interface IMigrationRunner
    {
        /// <summary>
        /// Updates the Database to Latest Migration.
        /// </summary>
        void UpdateToLatest();

        /// <summary>
        /// Updates to a specific version.
        /// </summary>
        /// <param name="updateToVersion">The update to version.</param>
        void UpdateTo(MigrationVersion updateToVersion);
    }
}