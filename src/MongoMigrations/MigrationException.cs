using System;

namespace MongoMigrations
{
    /// <summary>
    /// Exception that occurred during a migration
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class MigrationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationException"/> class.
        /// </summary>
        public MigrationException()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MigrationException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public MigrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}