using System;

// ReSharper disable once CheckNamespace
namespace Npgsql.EntityFrameworkCore.PostgreSQL
{
    internal static class VersionExtensions
    {
        // Note: a null version is interpreted as the latest version and will always return true.
        internal static bool AtLeast(this Version version, int major, int minor = 0)
            => version is null || version >= new Version(major, minor);

        // Note: a null version is interpreted as the latest version and will always return false.
        internal static bool IsUnder(this Version version, int major, int minor = 0)
            => version != null && version < new Version(major, minor);
    }
}
