CREATE PROCEDURE sp_Semester_Create
    @Name NVARCHAR(100),
    @OrderNumber INT,
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Semester 
    (Name, OrderNumber, CourseId, IsActive, CreatedAt, IsDeleted)
    VALUES 
    (@Name, @OrderNumber, @CourseId, 1, GETUTCDATE(), 0);

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END