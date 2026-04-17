CREATE or alter PROCEDURE sp_Department_Create
    @Name NVARCHAR(100),
    @Description NVARCHAR(255),
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Departments 
    (Name, Description, IsActive, CreatedAt, CreatedBy, IsDeleted)
    VALUES 
    (@Name, @Description, 1, GETUTCDATE(), @CreatedBy, 0);

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END