;CREATE or alter PROCEDURE sp_Class_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Classes
    SET IsActive = 0
    WHERE Id = @Id;
END