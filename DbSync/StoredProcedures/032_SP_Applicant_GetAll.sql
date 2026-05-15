CREATE OR ALTER PROCEDURE sp_Applicant_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.Id AS UserId,
        u.FullName,
        u.Email,
        u.PhoneNumber
  
    FROM AspNetUsers u
    INNER JOIN AspNetUserRoles ur
        ON u.Id = ur.UserId
    INNER JOIN AspNetRoles r
        ON ur.RoleId = r.Id
    WHERE r.Name = 'Applicant'
   
END