using System;
using System.Linq;
using MongoDB.Driver;

namespace MongoMigrations
{
    /// <inheritdoc />
    public class MigrationTarget : IMigrationTarget
    {
        /// <summary>
        /// The database
        /// </summary>
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationTarget"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        public MigrationTarget(IMongoDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        /// <inheritdoc />
        public string ServerAddresses => string.Join(",", _database.Client.Cluster.Description.Servers.Select(s => s.EndPoint.ToString()));

        /// <inheritdoc />
        public string DatabaseName => _database.DatabaseNamespace.DatabaseName;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Updating server(s) \"{ServerAddresses}\" for database \"{DatabaseName}\"";
        }
    }
}