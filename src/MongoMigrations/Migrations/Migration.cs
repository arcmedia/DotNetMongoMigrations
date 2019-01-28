using MongoDB.Driver;

namespace MongoMigrations.Migrations
{
    /// <inheritdoc />
    public abstract class Migration : IMigration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Migration" /> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="description">The description.</param>
        protected Migration(MigrationVersion version, string description)
        {
            Version = version;
            Description = description;
        }

        /// <inheritdoc />
        public MigrationVersion Version { get; private set; }

        /// <inheritdoc />
        public string Description { get; private set; }

        /// <inheritdoc />
        public abstract void Update(IMongoDatabase database);
    }
}