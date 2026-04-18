CREATE PROCEDURE sp_Semester_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Semester
    WHERE IsActive = 1 AND IsDeleted = 0;
END