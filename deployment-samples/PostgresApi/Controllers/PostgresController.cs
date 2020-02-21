using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace PostgresApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostgresController : ControllerBase
    {
        // GET: api/Postgres
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var results = new List<string>();

            using (var conn = new NpgsqlConnection(Configuration.ConnectionString("postgres")))
            {
                conn.Open();

                // Retrieve all rows
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM employee async;";

                    using (var reader = cmd.ExecuteReaderAsync().GetAwaiter().GetResult())
                    {
                        while (reader.Read())
                        {
                            var values = new object[10];
                            int count = reader.GetValues(values);
                            var newstring = string.Join(", ", values.Take(count));
                            results.Add(newstring);
                        }
                    }
                }
            }

            return results;
        }
    }
}
