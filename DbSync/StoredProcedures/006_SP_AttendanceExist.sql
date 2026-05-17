CREATE OR ALTER PROCEDURE SP_AttendanceExists
    @AttendanceDate DATE,
    @ClassId INT,
    @SubjectId INT,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CASE
            WHEN COUNT(1) > 0 THEN 1
            ELSE 0
        END AS ExistsFlag
    FROM Attendance a
    INNER JOIN AttendenceDetails ad
        ON a.Id = ad.AttendanceId
    WHERE
        a.AttendanceDate = @AttendanceDate
        AND a.ClassId = @ClassId
        AND a.SubjectId = @SubjectId
        AND ad.StudentId = @StudentId
        AND a.IsDeleted = 0;
END;