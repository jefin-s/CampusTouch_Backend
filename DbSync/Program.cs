//using Dapper;
//using Microsoft.Data.SqlClient;

//var connection = new SqlConnection(
//    "server=localhost;database=CampusTouchDb;Trusted_Connection=True;TrustServerCertificate=True"
//);

//await connection.OpenAsync();

//// Ensure ScriptHistory exists
//await connection.ExecuteAsync(@"
//IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScriptHistory')
//BEGIN
//    CREATE TABLE ScriptHistory (
//        Id INT IDENTITY(1,1) PRIMARY KEY,
//        ScriptName NVARCHAR(255),
//        ExecutedOn DATETIME DEFAULT GETDATE()
//    );
//END;
//");

//var basePath = AppDomain.CurrentDomain.BaseDirectory;
//var scriptPath = Path.Combine(basePath, "UpgradeScripts");

//var files = Directory.GetFiles(scriptPath, "*.sql")
//                     .OrderBy(f => f)
//                     .ToList();

//foreach (var file in files)
//{
//    var scriptName = Path.GetFileName(file);

//    var executed = await connection.ExecuteScalarAsync<int>(
//        "SELECT COUNT(1) FROM ScriptHistory WHERE ScriptName = @name",
//        new { name = scriptName });

//    if (executed > 0)
//    {
//        Console.WriteLine($"Skipping: {scriptName}");
//        continue;
//    }

//    Console.WriteLine($"Executing: {scriptName}");

//    var script = File.ReadAllText(file);

//    using var transaction = connection.BeginTransaction();

//    try
//    {
//        await connection.ExecuteAsync(script, transaction);

//        await connection.ExecuteAsync(
//            "INSERT INTO ScriptHistory (ScriptName) VALUES (@name)",
//            new { name = scriptName },
//            transaction
//        );

//        transaction.Commit();
//    }
//    catch (Exception ex)
//    {
//        transaction.Rollback();
//        Console.WriteLine($"Error in {scriptName}: {ex.Message}");
//        throw;
//    }
//}

//Console.WriteLine("DB Sync Completed ✅");


using Dapper;
using Microsoft.Data.SqlClient;

var connection = new SqlConnection(
    "server=localhost;database=CampusTouchDb;Trusted_Connection=True;TrustServerCertificate=True"
);

await connection.OpenAsync();

// 🔹 Ensure ScriptHistory table exists
await connection.ExecuteAsync(@"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScriptHistory')
BEGIN
    CREATE TABLE ScriptHistory (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ScriptName NVARCHAR(255),
        ExecutedOn DATETIME DEFAULT GETDATE()
    );
END;
");

var basePath = AppDomain.CurrentDomain.BaseDirectory;
var scriptPath = Path.Combine(basePath, "UpgradeScripts");

// 🔹 Get all scripts in order
var files = Directory.GetFiles(scriptPath, "*.sql")
                     .OrderBy(f => f)
                     .ToList();

foreach (var file in files)
{
    var scriptName = Path.GetFileName(file);

    // 🔹 Check if already executed
    var executed = await connection.ExecuteScalarAsync<int>(
        "SELECT COUNT(1) FROM ScriptHistory WHERE ScriptName = @name",
        new { name = scriptName }
    );

    if (executed > 0)
    {
        Console.WriteLine($"Skipping: {scriptName}");
        continue;
    }

    Console.WriteLine($"Executing: {scriptName}");

    var script = File.ReadAllText(file);

    try
    {
        // 🔹 Execute SQL file
        await connection.ExecuteAsync(script);

        // 🔹 Save execution history
        await connection.ExecuteAsync(
            "INSERT INTO ScriptHistory (ScriptName) VALUES (@name)",
            new { name = scriptName }
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error in {scriptName}: {ex.Message}");
        throw;
    }
}

Console.WriteLine("DB Sync Completed ✅");

//using Dapper;
//using Microsoft.Data.SqlClient;

//var connection = new SqlConnection(
//    "server=localhost;database=CampusTouchDb;Trusted_Connection=True;TrustServerCertificate=True"
//);

//await connection.OpenAsync();

//// Ensure ScriptHistory exists
//await connection.ExecuteAsync(@"
//IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScriptHistory')
//BEGIN
//    CREATE TABLE ScriptHistory (
//        Id INT IDENTITY(1,1) PRIMARY KEY,
//        ScriptName NVARCHAR(255),
//        ExecutedOn DATETIME DEFAULT GETDATE()
//    );
//END;
//");

//var basePath = AppDomain.CurrentDomain.BaseDirectory;
//var scriptPath = Path.Combine(basePath, "UpgradeScripts");

//var files = Directory.GetFiles(scriptPath, "*.sql")
//                     .OrderBy(f => f)
//                     .ToList();

//foreach (var file in files)
//{
//    var scriptName = Path.GetFileName(file);

//    var executed = await connection.ExecuteScalarAsync<int>(
//        "SELECT COUNT(1) FROM ScriptHistory WHERE ScriptName = @name",
//        new { name = scriptName }
//    );

//    if (executed > 0)
//    {
//        Console.WriteLine($"Skipping: {scriptName}");
//        continue;
//    }

//    Console.WriteLine($"Executing: {scriptName}");

//    var script = File.ReadAllText(file);

//    using var transaction = connection.BeginTransaction();

//    try
//    {
//        await connection.ExecuteAsync(script, transaction);

//        await connection.ExecuteAsync(
//            "INSERT INTO ScriptHistory (ScriptName) VALUES (@name)",
//            new { name = scriptName },
//            transaction
//        );

//        transaction.Commit();
//    }
//    catch (Exception ex)
//    {
//        transaction.Rollback();
//        Console.WriteLine($"❌ Error in {scriptName}: {ex.Message}");
//        throw;
//    }
//}

//Console.WriteLine("DB Sync Completed ✅");