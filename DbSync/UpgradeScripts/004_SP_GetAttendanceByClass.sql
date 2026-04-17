;CREATE OR ALTER PROCEDURE SP_GetAttendenceByClass
    @classid INT,
    @date DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ad.StudentId, 
        (s.FirstName + ' ' + s.LastName) AS StudentName, 
        ad.Status 
    FROM Attendance a
    JOIN AttendenceDetails ad ON ad.AttendanceId = a.Id
    JOIN Students s ON s.Id = ad.StudentId
    WHERE a.ClassId = @classid
      AND a.AttendanceDate = @date
      AND a.IsDeleted = 0;
END;