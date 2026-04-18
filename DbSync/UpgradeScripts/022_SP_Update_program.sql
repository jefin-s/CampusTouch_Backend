CREATE PROCEDURE sp_Program_Update
    @Id INT,
    @Name NVARCHAR(100),
    @Level NVARCHAR(50),
    @Duration INT,
    @DepartmentId INT,
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Courses
    SET Name = @Name,
        Level = @Level,
        Duration = @Duration,
        DepartmentId = @DepartmentId,
        UpdatedBy = @UpdatedBy,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END