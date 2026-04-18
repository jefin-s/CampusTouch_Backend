CREATE PROCEDURE sp_Program_Create
    @Name NVARCHAR(100),
    @Level NVARCHAR(50),
    @Duration INT,
    @DepartmentId INT,
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Courses 
    (Name, Level, Duration, DepartmentId, CreatedBy, CreatedAt, IsActive, IsDeleted)
    VALUES 
    (@Name, @Level, @Duration, @DepartmentId, @CreatedBy, GETUTCDATE(), 1, 0);

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END