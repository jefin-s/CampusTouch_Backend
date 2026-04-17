CREATE or alter PROCEDURE sp_Department_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Departments
    WHERE IsActive = 1 AND IsDeleted = 0
    ORDER BY Id;
END