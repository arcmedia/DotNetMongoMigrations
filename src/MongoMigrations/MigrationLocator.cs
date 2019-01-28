using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoMigrations.Contiguration;
using MongoMigrations.Filters;
using MongoMigrations.Migrations;

namespace MongoMigrations
{
    /// <summary>
    /// Locates migrations
    /// </summary>
    public class MigrationLocator : IMigrationLocator
    {
        /// <summary>
        /// The assemblies on which the locator will look for migrations
        /// </summary>
        private readonly List<Assembly> _assemblies = new List<Assembly>();

        /// <summary>
        /// The migration filters that will be applied when looking for migrations
        /// </summary>
        private readonly List<IMigrationFilter> _migrationFilters = new List<IMigrationFilter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationLocator" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public MigrationLocator(MongoMigrationConfiguration configuration)
        {
            _migrationFilters.Add(new ExcludeExperimentalMigrationsFilter());
            _assemblies.AddRange(configuration.Assemblies);
            _migrationFilters.AddRange(configuration.MigrationFilters);
        }

        /// <inheritdoc />
        public virtual void LookForMigrationsInAssemblyOfType<T>()
        {
            var assembly = typeof(T).Assembly;
            LookForMigrationsInAssembly(assembly);
        }

        /// <inheritdoc />
        public void LookForMigrationsInAssembly(Assembly assembly)
        {
            if (_assemblies.Contains(assembly))
            {
                return;
            }

            _assemblies.Add(assembly);
        }

        /// <inheritdoc />
        public virtual IEnumerable<IMigration> GetAllMigrations()
        {
            return _assemblies
                .SelectMany(GetMigrationsFromAssembly)
                .OrderBy(m => m.Version);
        }

        /// <inheritdoc />
        public virtual MigrationVersion LatestVersion()
        {
            if (!GetAllMigrations().Any())
            {
                return MigrationVersion.Default();
            }

            return GetAllMigrations()
                .Max(m => m.Version);
        }

        /// <inheritdoc />
        public virtual IEnumerable<IMigration> GetMigrationsAfter(IAppliedMigration currentVersion)
        {
            var migrations = GetAllMigrations();

            if (currentVersion != null)
            {
                migrations = migrations.Where(m => m.Version > currentVersion.Version);
            }

            return migrations.OrderBy(m => m.Version);
        }

        /// <inheritdoc />
        public void AddMigrationFilter(IMigrationFilter filter)
        {
            _migrationFilters.Add(filter);
        }

        /// <summary>
        /// Gets the migrations from assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>All migrations in the specific assembly</returns>
        protected virtual IEnumerable<IMigration> GetMigrationsFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes()
                    .Where(t => typeof(IMigration).IsAssignableFrom(t) && !t.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .OfType<IMigration>()
                    .Where(m => !_migrationFilters.Any(f => f.Exclude(m)));
            }
            catch (Exception exception)
            {
                throw new MigrationException($"Cannot load migrations from assembly: assembly.FullName", exception);
            }
        }
    }
}