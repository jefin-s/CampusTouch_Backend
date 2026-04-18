CREATE PROCEDURE sp_Program_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Courses
    WHERE Id = @Id AND IsDeleted = 0;
END