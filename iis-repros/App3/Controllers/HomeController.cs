using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App3.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true";
        private readonly string _tableName = "App3Table";

        public ActionResult Index()
        {
            // When loading the page, just try to create a connection to the database and create a table if it doesn't yet exist
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = $"IF NOT EXISTS( SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{_tableName}') " +
                                      $"CREATE TABLE {_tableName} (Id int identity(1,1),Text VARCHAR(500))";
                command.ExecuteNonQuery();
                connection.Close();
                Console.WriteLine($"Created table {_tableName} in AppDomain {AppDomain.CurrentDomain.FriendlyName}.");
            }

            // Read values
            using (var connection = (DbConnection)new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                Console.WriteLine($"Reading last row from {_tableName}");
                command.CommandText = $"SELECT TOP 1 Text FROM {_tableName} ORDER BY Id DESC;";
                connection.Open();
                var reader = command.ExecuteReader();
                reader.Read();

                if (reader.HasRows)
                {
                    var result = reader[0];
                    Console.WriteLine(result.ToString());
                }
            }

            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
