using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgresApi
{
    internal static class Configuration
    {
        internal static string ConnectionString(string database)
        {
            return $"Host={Host()};Username={Username()};Password={Password()};Database={database}";
        }

        private static string Host()
        {
            return Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "postgres";
        }

        private static string Username()
        {
            return Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
        }

        private static string Password()
        {
            return Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";
        }
    }
}
