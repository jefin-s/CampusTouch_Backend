;CREATE or alter PROCEDURE sp_Class_Create
    @Name NVARCHAR(100),
    @DepartmentId INT,
    @CourseId INT,
    @Year INT,
    @Semester INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Classes (Name, DepartmentId, CourseId, Year, Semester)
    VALUES (@Name, @DepartmentId, @CourseId, @Year, @Semester);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END