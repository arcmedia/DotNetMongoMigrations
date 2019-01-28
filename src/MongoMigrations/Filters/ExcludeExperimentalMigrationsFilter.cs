using System.Linq;
using MongoMigrations.Filters;
using MongoMigrations.Migrations;

namespace MongoMigrations
{
    /// <summary>
    /// Filters experimental migrations
    /// </summary>
    /// <seealso cref="MigrationFilter" />
    public class ExcludeExperimentalMigrationsFilter : MigrationFilter
    {
        /// <inheritdoc />
        public override bool Exclude(IMigration migration)
        {
            if (migration == null)
            {
                return false;
            }

            return migration.GetType()
                .GetCustomAttributes(true)
                .OfType<ExperimentalMigrationAttribute>()
                .Any();
        }
    }
}