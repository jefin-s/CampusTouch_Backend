CREATE or alter PROCEDURE sp_Department_Create
    @Name NVARCHAR(100),
    @Description NVARCHAR(255),
    @CreatedBy NVARCHAR(100),
    @Code NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Departments 
    (Name, Description, IsActive, CreatedAt, CreatedBy, IsDeleted,Code)
    VALUES 
    (@Name, @Description, 1, GETUTCDATE(), @CreatedBy, 0,@Code);
        
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END