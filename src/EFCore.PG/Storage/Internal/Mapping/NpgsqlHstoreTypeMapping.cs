﻿using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using NpgsqlTypes;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping
{
    /// <summary>
    /// The type mapping for the PostgreSQL hstore type.
    /// </summary>
    /// <remarks>
    /// See: https://www.postgresql.org/docs/current/static/hstore.html
    /// </remarks>
    public class NpgsqlHstoreTypeMapping : NpgsqlTypeMapping
    {
        static readonly HstoreComparer ComparerInstance = new HstoreComparer();

        /// <summary>
        /// Constructs an instance of the <see cref="NpgsqlHstoreTypeMapping"/> class.
        /// </summary>
        public NpgsqlHstoreTypeMapping()
            : base(
                new RelationalTypeMappingParameters(
                    new CoreTypeMappingParameters(typeof(Dictionary<string, string>), null, ComparerInstance),
                    "hstore"
                ), NpgsqlDbType.Hstore) {}

        protected NpgsqlHstoreTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters, NpgsqlDbType.Hstore) {}

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new NpgsqlHstoreTypeMapping(parameters);

        protected override string GenerateNonNullSqlLiteral(object value)
        {
            var sb = new StringBuilder("HSTORE '");
            foreach (var kv in (Dictionary<string, string>)value)
            {
                sb.Append('"');
                sb.Append(kv.Key);   // TODO: Escape
                sb.Append("\"=>");
                if (kv.Value == null)
                    sb.Append("NULL");
                else
                {
                    sb.Append('"');
                    sb.Append(kv.Value);   // TODO: Escape
                    sb.Append("\",");
                }
            }

            sb.Remove(sb.Length - 1, 1);

            sb.Append('\'');
            return sb.ToString();
        }

        class HstoreComparer : ValueComparer<Dictionary<string, string>>
        {
            public HstoreComparer() : base(
                (a, b) => Compare(a,b),
                o => o.GetHashCode(),
                o => o == null ? null : new Dictionary<string, string>(o))
            {}

            static bool Compare(Dictionary<string, string> a, Dictionary<string, string> b)
            {
                if (a == null)
                    return b == null;
                if (b == null)
                    return false;
                if (a.Count != b.Count)
                    return false;
                foreach (var kv in a)
                    if (!b.TryGetValue(kv.Key, out var bValue) || kv.Value != bValue)
                        return false;
                return true;
            }
        }
    }
}
