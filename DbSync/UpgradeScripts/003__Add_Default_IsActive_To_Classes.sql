IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints
    WHERE name = 'DF_Classes_IsActive'
)
BEGIN
    ALTER TABLE Classes
    ADD CONSTRAINT DF_Classes_IsActive
    DEFAULT 1 FOR IsActive;
END

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE Name = N'IsActive'
    AND Object_ID = Object_ID(N'Classes')
)
BEGIN
    ALTER TABLE Classes
    ALTER COLUMN IsActive BIT NOT NULL;
END