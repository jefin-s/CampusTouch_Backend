CREATE PROCEDURE sp_Semester_Exists
    @CourseId INT,
    @OrderNumber INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM Semester
    WHERE CourseId = @CourseId
      AND OrderNumber = @OrderNumber
      AND IsDeleted = 0;
END