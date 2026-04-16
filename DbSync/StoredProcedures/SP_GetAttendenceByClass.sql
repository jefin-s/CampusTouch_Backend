create  procedure SP_GetAttendenceByClass
  @classid int,
  @date date
  as
   begin 
   SELECT 
    ad.StudentId, 
    (s.FirstName + ' ' + s.LastName) AS StudentName, 
    ad.Status 
FROM Attendance a
JOIN AttendenceDetails ad ON ad.AttendanceId = a.Id
JOIN Students s ON s.Id = ad.StudentId
WHERE a.ClassId = @classid
AND CAST(a.AttendanceDate AS DATE) = CAST(@date AS DATE)
AND a.IsDeleted = 0;
   end
