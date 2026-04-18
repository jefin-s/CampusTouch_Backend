CREATE PROCEDURE sp_Semester_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Semester
    WHERE Id = @Id 
      AND IsActive = 1 
      AND IsDeleted = 0;
END