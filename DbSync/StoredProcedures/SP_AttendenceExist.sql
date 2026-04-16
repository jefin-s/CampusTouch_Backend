create procedure SP_AttendenceExist
	 @date date,
	 @ClassId int,
	 @SubjectId int
	 as
	 begin 
	 SELECT COUNT(1)
            FROM Attendance
            WHERE AttendanceDate = @date
              AND ClassId = @classId
              AND SubjectId = @subjectId
              AND IsDeleted = 0;
	 end
