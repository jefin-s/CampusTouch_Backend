using CampusTouch.Application.Features.Students.DTOs;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using Dapper;
using System.Data;


namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public class StudentRepository : IStudentRepository
    {

        private readonly IDbConnection _dbConnection;
        public StudentRepository(IDbConnection dbConnection)
        {

            _dbConnection = dbConnection;
        }

        public async Task<(IEnumerable<Student> Students, int TotalCount)> GetAllStudents(
        int pageNumber,
        int pageSize,
        string? Search)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var sql = @"
        SELECT *
        FROM Students
        WHERE IsActive = 1
        AND (
            @Search IS NULL
            OR (FirstName + ' ' + LastName) LIKE '%' + @Search + '%'
        )
        ORDER BY Id
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(*)
        FROM Students
        WHERE IsActive = 1
        AND (
            @Search IS NULL
            OR (FirstName + ' ' + LastName) LIKE '%' + @Search + '%'
        );
    ";

            var parameters = new
            {
                Search,
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            };

            using var multi =
                await _dbConnection.QueryMultipleAsync(sql, parameters);

            var students =
                await multi.ReadAsync<Student>();

            var totalCount =
                await multi.ReadFirstAsync<int>();

            return (students.ToList(), totalCount);
        }
        public async Task<Student?> GetStudentsById(int id)
        {
            var query = "Select *from students  where id=@id";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Student>(query, new { id = id });
            return result;

        }
        public async Task<int> CreateStudentAsync(Student student)
        {
            var sql = @"INSERT INTO Students
                        (UserId, AdmissionNumber,
                         FirstName, LastName,
                         DateOfBirth, Gender,
                         PhoneNumber, Email, Address,
                         CourseId, DepartmentId, AdmissionDate,
                         GuardianName, GuardianPhone,
                         BloodGroup, ProfileImageUrl,
                         IsActive, CreatedAt)
                        VALUES
                        ( @UserId, @AdmissionNumber,
                         @FirstName, @LastName,
                         @DateOfBirth, @Gender,
                         @PhoneNumber, @Email, @Address,
                         @CourseId, @DepartmentId, @AdmissionDate,

                         @GuardianName, @GuardianPhone,
                         @BloodGroup, @ProfileImageUrl,
                         @IsActive, @CreatedAt)";

            var result = await _dbConnection.ExecuteAsync(sql, student);

            return result;
        }

        public async Task<bool> AdmissionNumberExist(string admissionNumber)
        {
            var query = "select count(1) from students where AdmissionNumber=@AdmissionNumber and  isdeleted=0";

            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new
            {
                AdmissionNumber = admissionNumber
            });

            return count > 0;

        }
        public async Task<bool> UpdateStudent(Student student)
        {
            var query = @"UPDATE Students SET
                    AdmissionNumber = @AdmissionNumber,
                    CourseId = @CourseId,
                    DepartmentId = @DepartmentId,
                    AdmissionDate = @AdmissionDate,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    DateOfBirth = @DateOfBirth,
                    Gender = @Gender,
                    PhoneNumber = @PhoneNumber,
                    Email = @Email,
                    Address = @Address,
                    GuardianName = @GuardianName,
                    GuardianPhone = @GuardianPhone,
                    BloodGroup = @BloodGroup,
                    ProfileImageUrl = @ProfileImageUrl,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                  WHERE Id = @Id AND IsDeleted = 0";

            var rowsAffected = await _dbConnection.ExecuteAsync(query, student);

            return rowsAffected > 0;
        }
        public async Task<bool> DeleteStudent(int id, string userid)
        {
            var query = @"UPDATE Students 
                   SET IsDeleted = 1,
                        isactive=0,
                       DeletedAt = GETUTCDATE(),
                       DeletedBy = @UserId
                   WHERE Id = @id";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = id, UserId = userid });
            return rowsAffected > 0;
        }

        public async Task<int> GetNextAdmissionSequence(int deptId)
        {
            var year = DateTime.UtcNow.Year;

            var sql = @"
SELECT ISNULL(MAX(CAST(RIGHT(AdmissionNumber, 3) AS INT)), 0)
FROM Students
WHERE DepartmentId = @DepartmentId
AND YEAR(CreatedAt) = @Year";

            var maxSequence = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                DepartmentId = deptId,
                Year = year
            });

            return maxSequence + 1;
        }

        public async Task<StudentMyProfileDTO> GetStudentByUserId(string userId)
        {
            var query = @"
SELECT 
    s.Id,
    s.FirstName,
    s.LastName,
    s.Email,
    s.PhoneNumber,
    s.Address,
    s.Gender,
    s.DateOfBirth,
    s.AdmissionDate,
    s.BloodGroup,
    s.ProfileImageUrl,

    s.CourseId,
    c.Name AS CourseName,

    s.DepartmentId,
    d.Name AS DepartmentName

FROM Students s
LEFT JOIN Courses c ON s.CourseId = c.Id
LEFT JOIN Departments d ON s.DepartmentId = d.Id

WHERE s.UserId = @UserId AND s.IsDeleted = 0
";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<StudentMyProfileDTO>(query, new { UserId = userId });
            return result;
        }

        //public async Task<List<Student>> GetStudentbyName(string query)
        //{
        //    string sql = @"
        //    SELECT TOP 20
        //        Id,
        //        Name,
        //        Email,
        //        StudentId,
        //        Department,
        //        ProfileImageUrl
        //    FROM Students
        //    WHERE Name LIKE @Search
        //";

        //    var students = await _dbConnection.QueryAsync<Student>(
        //        sql,
        //        new
        //        {
        //            Search = $"%{query}%"
        //        });

        //    return students.ToList();
        //}
    }
}
    