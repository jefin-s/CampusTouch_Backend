;CREATE or alter PROCEDURE sp_Class_Exists
    @CourseId INT,
    @Year INT,
    @Semester INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1) 
    FROM Classes
    WHERE CourseId = @CourseId 
      AND Year = @Year 
      AND Semester = @Semester;
END