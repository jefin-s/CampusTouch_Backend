CREATE OR ALTER PROCEDURE SP_GetAttendanceId
(
    @AttendanceDate DATE,
    @ClassId INT,
    @SubjectId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id
    FROM Attendance
    WHERE AttendanceDate = @AttendanceDate
        AND ClassId = @ClassId
        AND SubjectId = @SubjectId
        AND IsDeleted = 0;
END