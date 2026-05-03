;CREATE OR ALTER PROCEDURE SP_GetAttendanceByStudentId
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        a.AttendanceDate AS AttendanceDate,
        sub.Name AS SubjectName,
        ad.Status
    FROM Attendance a
    JOIN AttendenceDetails ad ON ad.AttendanceId = a.Id
    JOIN Subjects sub ON sub.Id = a.SubjectId
    WHERE ad.StudentId = @StudentId
      AND a.IsDeleted = 0
    ORDER BY a.AttendanceDate DESC;
END;