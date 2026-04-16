create  procedure SP_CreateAttendence
	 @AttendenceDate date,
	 @ClassId int,
	 @SubjectId int,
	 @StaffId nvarchar(450)
	 as
	 begin
	   INSERT INTO Attendance (AttendanceDate, ClassId, SubjectId, StaffId)
            VALUES (@AttendanceDate, @ClassId, @SubjectId, @StaffId);

            SELECT CAST(SCOPE_IDENTITY() as int);
	 end;