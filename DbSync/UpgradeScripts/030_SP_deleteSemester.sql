CREATE PROCEDURE sp_Semester_Delete
    @Id INT,
    @UserId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Semester
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedAt = GETUTCDATE(),
        DeletedBy = @UserId
    WHERE Id = @Id;
END