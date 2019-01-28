using System.Collections.Generic;
using System.Reflection;
using MongoMigrations.Filters;

namespace MongoMigrations.Contiguration
{
    /// <summary>
    /// Configuration for Mongo Migration
    /// </summary>
    public class MongoMigrationConfiguration
    {
        /// <summary>
        /// The assemblies that will be used for migration location
        /// </summary>
        private readonly List<Assembly> _assemblies = new List<Assembly>();

        /// <summary>
        /// The migration filters that will be applied for migration location
        /// </summary>
        private readonly List<IMigrationFilter> _migrationFilters = new List<IMigrationFilter>();

        /// <summary>
        /// Gets the assemblies that will be used for migration location.
        /// </summary>
        public IReadOnlyCollection<Assembly> Assemblies => _assemblies;

        /// <summary>
        /// Gets the migration filters that will be applied for migration location.
        /// </summary>
        public IReadOnlyCollection<IMigrationFilter> MigrationFilters => _migrationFilters;

        /// <summary>
        /// Adds an assembly for migration location.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public MongoMigrationConfiguration AddAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
            return this;
        }

        /// <summary>
        /// Adds the migration filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public MongoMigrationConfiguration AddMigrationFilter(IMigrationFilter filter)
        {
            _migrationFilters.Add(filter);
            return this;
        }
    }
}