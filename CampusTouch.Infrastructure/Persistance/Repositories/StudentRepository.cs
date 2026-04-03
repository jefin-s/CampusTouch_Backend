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

        public async Task<IEnumerable<Student>> GetAllStudents(int pageNumber, int pageSize, string? Search)
        {

            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize > 100 ? 100 : pageSize;
            var sql = @"select *from students  where isactive=1 
            AND (@Search is null or  ( firstname+' '+lastname )like '%'+@Search+'%')
           order by id  OFFSET @Offset ROWS FETCH NEXT  @PageSize ROWS ONLY";
            var parameters = new
            {
                Search = Search,
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            };
            var data = await _dbConnection.QueryAsync<Student>(sql, parameters);
            return data.ToList();


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
            var query = "select count(1) from students where AdmissionNumber=@admissionNumber where is isdeleted=0";

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
                   WHERE Id = @Id";
            var rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = id, UserId = userid });
            return rowsAffected > 0;
        }
    }
}
    