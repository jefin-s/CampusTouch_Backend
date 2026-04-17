;CREATE or alter  PROCEDURE sp_Class_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * 
    FROM Classes 
    WHERE IsActive = 1;
END