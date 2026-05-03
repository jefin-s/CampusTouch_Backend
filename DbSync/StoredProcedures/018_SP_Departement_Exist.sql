CREATE or  alter  PROCEDURE sp_Department_Exists
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM Departments
    WHERE Name = @Name AND IsDeleted = 0;
END