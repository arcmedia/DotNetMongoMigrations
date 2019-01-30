namespace MongoMigrations
{
    /// <summary>
    /// Migration Target
    /// </summary>
    public interface IMigrationTarget
    {
        /// <summary>
        /// Gets the server addresses.
        /// </summary>
        string ServerAddresses { get; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        string DatabaseName { get; }
    }
}