CREATE PROCEDURE sp_Semester_Update
    @Id INT,
    @Name NVARCHAR(100),
    @OrderNumber INT,
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Semester
    SET Name = @Name,
        OrderNumber = @OrderNumber,
        CourseId = @CourseId,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id 
      AND IsActive = 1;
END