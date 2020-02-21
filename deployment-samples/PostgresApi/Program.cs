using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace PostgresApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SeedDatabase();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static void SeedDatabase()
        {
            using (var conn = new NpgsqlConnection(Configuration.ConnectionString("postgres")))
            {
                conn.Open();

                // Re-create the table each time
                using (var createTableCmd = conn.CreateCommand())
                {
                    createTableCmd.CommandText = @"
        DROP TABLE IF EXISTS employee;
        CREATE TABLE employee (
            employee_id SERIAL,
            name varchar(45) NOT NULL,
            birth_date varchar(450) NOT NULL,
          PRIMARY KEY (employee_id)
        )";

                    createTableCmd.ExecuteNonQuery();
                }

                // Insert some data
                using (var insertCmd = conn.CreateCommand())
                {
                    insertCmd.CommandText = "INSERT INTO employee (name, birth_date) VALUES (@name, @birth_date);";
                    insertCmd.Parameters.AddWithValue("name", "Jane Smith");
                    insertCmd.Parameters.AddWithValue("@birth_date", new DateTime(1980, 2, 3));

                    insertCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
