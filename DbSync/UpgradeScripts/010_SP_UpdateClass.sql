;CREATE or alter PROCEDURE sp_Class_Update
    @Id INT,
    @Name NVARCHAR(100),
    @DepartmentId INT,
    @CourseId INT,
    @Year INT,
    @Semester INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Classes
    SET Name = @Name,
        DepartmentId = @DepartmentId,
        CourseId = @CourseId,
        Year = @Year,
        Semester = @Semester
    WHERE Id = @Id;
END