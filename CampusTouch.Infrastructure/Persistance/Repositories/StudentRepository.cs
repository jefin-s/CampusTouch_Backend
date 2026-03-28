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

        public async Task<IEnumerable<Student>> GetAllStudents(int pageNumber,int pageSize,string? Search)
        {

            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize > 100 ? 100 : pageSize;
            var sql =   @"select *from students  where isactive=1 
            AND (@Search is null or  ( firstname+' '+lastname )like '%'+@Search+'%')
           order by id  OFFSET @Offset ROWS FETCH NEXT  @PageSize ROWS ONLY";
            var parameters = new
            {
                Search = Search,
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            };
            var data = await _dbConnection.QueryAsync<Student>(sql,parameters);
            return data.ToList();
                

        }
        public async Task<Student?> GetStudentsById(int id)
        {
            var query = "Select *from students  where id=@id";
            var result=await _dbConnection.QueryFirstOrDefaultAsync<Student>(query, new { id=id});
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
    }   
}
    