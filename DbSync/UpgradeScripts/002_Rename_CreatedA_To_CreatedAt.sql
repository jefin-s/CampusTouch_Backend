IF EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE Name = 'CreatedA' 
    AND Object_ID = Object_ID('dbo.Semester')
)
BEGIN
    EXEC sp_rename 'dbo.Semester.CreatedA', 'CreatedAt', 'COLUMN';
END