using System;

namespace MongoMigrations
{
    /// <summary>
    /// Marks a migration as Experimental, and should not be applied
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ExperimentalMigrationAttribute : Attribute
    {
    }
}