create procedure SP_GetAttendenceByStudentID
@studentID int
as
begin 
 SELECT 
        a.AttendanceDate AS Date,
        sub.Name AS SubjectName,
        ad.Status
    FROM Attendance a
    JOIN AttendenceDetails ad ON ad.AttendanceId = a.Id
    JOIN Subjects sub ON sub.Id = a.SubjectId
    WHERE ad.StudentId = @studentId
      AND a.IsDeleted = 0
    ORDER BY a.AttendanceDate DESC
end