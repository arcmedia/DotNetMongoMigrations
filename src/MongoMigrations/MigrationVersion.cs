using System;

namespace MongoMigrations
{
    /// <summary>
    /// Represents a migration version
    /// </summary>
    public struct MigrationVersion : IComparable<MigrationVersion>, IEquatable<MigrationVersion>
    {
        /// <summary>
        /// The major part of the version
        /// </summary>
        private readonly int _major;

        /// <summary>
        /// The minor part of the version
        /// </summary>
        private readonly int _minor;

        /// <summary>
        /// The revision part of the version
        /// </summary>
        private readonly int _revision;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationVersion"/> struct.
        /// </summary>
        /// <param name="version">The version.</param>
        public MigrationVersion(string version)
        {
            var versionParts = version.Split('.');
            if (versionParts.Length != 3)
            {
                throw new ArgumentException("Versions must have format: major.minor.revision, this doesn't match: " +
                                            version);
            }

            var majorString = versionParts[0];
            if (!int.TryParse(majorString, out _major))
            {
                throw new ArgumentException("Invalid major version value: " + majorString);
            }

            var minorString = versionParts[1];
            if (!int.TryParse(minorString, out _minor))
            {
                throw new ArgumentException("Invalid major version value: " + minorString);
            }

            var revisionString = versionParts[2];
            if (!int.TryParse(revisionString, out _revision))
            {
                throw new ArgumentException("Invalid major version value: " + revisionString);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationVersion"/> struct.
        /// </summary>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        /// <param name="revision">The revision.</param>
        public MigrationVersion(int major, int minor, int revision)
        {
            _major = major;
            _minor = minor;
            _revision = revision;
        }

        /// <summary>
        /// Gets the major part of the version.
        /// </summary>
        public int Major => _major;

        /// <summary>
        /// Gets the minor part of the version.
        /// </summary>
        public int Minor => _minor;

        /// <summary>
        /// Gets the revision part of the version.
        /// </summary>
        public int Revision => _revision;

        /// <summary>
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="MigrationVersion"/>.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator MigrationVersion(string version)
        {
            return ToMigrationVersion(version);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="MigrationVersion"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(MigrationVersion version)
        {
            return version.ToString();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(MigrationVersion a, MigrationVersion b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(MigrationVersion a, MigrationVersion b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(MigrationVersion a, MigrationVersion b)
        {
            return a.Major > b.Major
                   || (a.Major == b.Major && a.Minor > b.Minor)
                   || (a.Major == b.Major && a.Minor == b.Minor && a.Revision > b.Revision);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(MigrationVersion a, MigrationVersion b)
        {
            return a != b && !(a > b);
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <=(MigrationVersion a, MigrationVersion b)
        {
            return a == b || a < b;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >=(MigrationVersion a, MigrationVersion b)
        {
            return a == b || a > b;
        }

        /// <summary>
        /// Return the default, "first" version 0.0.0
        /// </summary>
        public static MigrationVersion Default()
        {
            return default(MigrationVersion);
        }

        /// <summary>
        /// Converts a string to a Migration Version
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <returns>Migration Version</returns>
        public static MigrationVersion ToMigrationVersion(string version)
        {
            return new MigrationVersion(version);
        }

        /// <inheritdoc />
        public bool Equals(MigrationVersion other)
        {
            return other.Major == Major && other.Minor == Minor && other.Revision == Revision;
        }

        /// <inheritdoc />
        public int CompareTo(MigrationVersion other)
        {
            if (Equals(other))
            {
                return 0;
            }

            return this > other ? 1 : -1;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != typeof(MigrationVersion))
            {
                return false;
            }

            return Equals((MigrationVersion)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var result = Major;
                result = (result * 397) ^ Minor;
                result = (result * 397) ^ Revision;
                return result;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{_major}.{_minor}.{_revision}";
        }
    }
}