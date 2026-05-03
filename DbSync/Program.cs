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








//using Dapper;
//using Microsoft.Data.SqlClient;

//var connection = new SqlConnection(
//    "server=localhost;database=CampusTouchDb;Trusted_Connection=True;TrustServerCertificate=True"

//);

//await connection.OpenAsync();

//// 🔹 Ensure ScriptHistory table exists
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

//// 🔹 Get all scripts in order
//var files = Directory.GetFiles(scriptPath, "*.sql")
//                     .OrderBy(f => f)
//                     .ToList();

//foreach (var file in files)
//{
//    var scriptName = Path.GetFileName(file);

//    // 🔹 Check if already executed
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

//    try
//    {
//        // 🔹 Execute SQL file
//        await connection.ExecuteAsync(script);

//        // 🔹 Save execution history
//        await connection.ExecuteAsync(
//            "INSERT INTO ScriptHistory (ScriptName) VALUES (@name)",
//            new { name = scriptName }
//        );
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"❌ Error in {scriptName}: {ex.Message}");
//        throw;
//    }
//}

//Console.WriteLine("DB Sync Completed ✅");




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

using Dapper;
using Microsoft.Data.SqlClient;

var connectionString = "Server=database-1.cpc8w0g2uovg.ap-south-1.rds.amazonaws.com,1433;Database=CampusTouchDb;User Id=admin;Password=Jefinsathyekkal;TrustServerCertificate=True;";

using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();

// ✅ Ensure ScriptHistory table exists
await connection.ExecuteAsync(@"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScriptHistory')
BEGIN
    CREATE TABLE ScriptHistory (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ScriptName NVARCHAR(255) UNIQUE,
        ExecutedOn DATETIME2 DEFAULT GETUTCDATE()
    );
END;
");

var basePath = AppDomain.CurrentDomain.BaseDirectory;

// ✅ Execution order (VERY IMPORTANT)
string[] folders =
{
    "DB_Schema",
    "UpgradeScripts",
    "StoredProcedures"
};

foreach (var folder in folders)
{
    var scriptPath = Path.Combine(basePath, folder);

    if (!Directory.Exists(scriptPath))
    {
        Console.WriteLine($"⚠ Folder not found: {folder}");
        continue;
    }

    Console.WriteLine($"\n📂 Processing folder: {folder}");

    var files = Directory.GetFiles(scriptPath, "*.sql")
                         .OrderBy(f => f)
                         .ToList();

    foreach (var file in files)
    {
        var scriptName = Path.GetFileName(file);

        // ✅ Check if already executed
        var executed = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM ScriptHistory WHERE ScriptName = @name",
            new { name = scriptName }
        );

        if (executed > 0)
        {
            Console.WriteLine($"⏭ Skipping: {scriptName}");
            continue;
        }

        Console.WriteLine($"▶ Executing: {scriptName}");

        var script = await File.ReadAllTextAsync(file);

        // ✅ Split GO statements (IMPORTANT)
        var batches = script.Split(
            new[] { "\r\nGO\r\n", "\nGO\n", "\r\nGO\n", "\nGO\r\n" },
            StringSplitOptions.RemoveEmptyEntries
        );

        using var transaction = connection.BeginTransaction();

        try
        {
            foreach (var batch in batches)
            {
                if (!string.IsNullOrWhiteSpace(batch))
                {
                    await connection.ExecuteAsync(batch, transaction: transaction);
                }
            }

            // ✅ Save execution history
            await connection.ExecuteAsync(
                "INSERT INTO ScriptHistory (ScriptName) VALUES (@name)",
                new { name = scriptName },
                transaction
            );

            transaction.Commit();

            Console.WriteLine($"✅ Success: {scriptName}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            Console.WriteLine($"❌ Error in {scriptName}: {ex.Message}");
            throw;
        }
    }
}

Console.WriteLine("\n🎉 DB Sync Completed Successfully");