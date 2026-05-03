;CREATE OR ALTER PROCEDURE SP_AttendanceExists
    @AttendanceDate DATE,
    @ClassId INT,
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CASE 
            WHEN COUNT(1) > 0 THEN 1 
            ELSE 0 
        END AS ExistsFlag
    FROM Attendance
    WHERE AttendanceDate = @AttendanceDate
      AND ClassId = @ClassId
      AND SubjectId = @SubjectId
      AND IsDeleted = 0;
END;