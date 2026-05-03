;CREATE or alter PROCEDURE sp_Class_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * 
    FROM Classes 
    WHERE Id = @Id AND IsActive = 1;
END