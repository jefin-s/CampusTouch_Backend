CREATE  or alter  PROCEDURE sp_Department_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Departments
    SET IsActive = 0,
        IsDeleted = 1,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END