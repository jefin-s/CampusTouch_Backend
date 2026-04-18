CREATE PROCEDURE sp_Program_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Courses
    WHERE IsActive = 1 AND IsDeleted = 0;
END