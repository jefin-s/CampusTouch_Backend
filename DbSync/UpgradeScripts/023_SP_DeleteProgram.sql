CREATE PROCEDURE sp_Program_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Courses
    SET IsActive = 0,
        IsDeleted = 1,
        DeletedAt = GETUTCDATE()
    WHERE Id = @Id;
END