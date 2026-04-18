CREATE PROCEDURE sp_Program_Exists
    @Name NVARCHAR(100),
    @DepartmentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM Courses
    WHERE Name = @Name 
      AND DepartmentId = @DepartmentId
      AND IsActive = 1;
END