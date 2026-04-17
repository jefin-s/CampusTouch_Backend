CREATE or alter PROCEDURE sp_Department_Update
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Departments
    SET Name = @Name,
        Description = @Description,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END