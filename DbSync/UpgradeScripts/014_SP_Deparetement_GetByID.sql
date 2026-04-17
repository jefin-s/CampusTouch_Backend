CREATE or alter  PROCEDURE sp_Department_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Departments
    WHERE Id = @Id AND IsDeleted = 0;
END