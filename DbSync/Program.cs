
using Dapper;
using Microsoft.Data.SqlClient;

var connection = new SqlConnection("server=localhost;database=CampusTouchDb;Trusted_Connection=True;TrustServerCertificate=True");

var basePath = AppDomain.CurrentDomain.BaseDirectory;
var tablePath = Path.Combine(basePath, "Tables");

var files = Directory.GetFiles(tablePath, "*.sql");

foreach (var file in files)
{
    var sql = File.ReadAllText(file);
    await connection.ExecuteAsync(sql);
}

Console.WriteLine("DB Sync Completed");