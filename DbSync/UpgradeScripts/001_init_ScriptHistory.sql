;IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScriptHistory')
BEGIN
    CREATE TABLE ScriptHistory (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ScriptName NVARCHAR(255),
        ExecutedOn DATETIME DEFAULT GETDATE()
    );
END;