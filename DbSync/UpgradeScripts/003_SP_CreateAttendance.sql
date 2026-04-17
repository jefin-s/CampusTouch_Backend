;CREATE OR ALTER PROCEDURE SP_CreateAttendance
    @AttendanceDate DATE,
    @ClassId INT,
    @SubjectId INT,
    @StaffId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Attendance (AttendanceDate, ClassId, SubjectId, StaffId)
    OUTPUT INSERTED.Id
    VALUES (@AttendanceDate, @ClassId, @SubjectId, @StaffId);
END;