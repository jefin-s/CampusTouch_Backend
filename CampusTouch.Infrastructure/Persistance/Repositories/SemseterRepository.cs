using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using Dapper;
using System.Data;

namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public class SemseterRepository : ISemsterRepository
    {
        private readonly IDbConnection _dbconnection;

        public SemseterRepository(IDbConnection dbConnection)
        {
            _dbconnection = dbConnection;
        }

  
        public async Task<int> CreateAsync(Semesters semester)
        {
            return await _dbconnection.ExecuteScalarAsync<int>(
                "sp_Semester_Create",
                new
                {
                    semester.Name,
                    semester.OrderNumber,
                    semester.CourseId
                },
                commandType: CommandType.StoredProcedure
            );
        }

      
        public async Task<IEnumerable<Semesters>> GetAllAsync()
        {
            return await _dbconnection.QueryAsync<Semesters>(
                "sp_Semester_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

  
        public async Task<Semesters?> GetByIdAsync(int id)
        {
            return await _dbconnection.QueryFirstOrDefaultAsync<Semesters>(
                "sp_Semester_GetById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

       
        public async Task<bool> UpdateAsync(Semesters semester)
        {
            var rows = await _dbconnection.ExecuteAsync(
                "sp_Semester_Update",
                new
                {
                    semester.Id,
                    semester.Name,
                    semester.OrderNumber,
                    semester.CourseId
                },
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

 
        public async Task<bool> DeleteAsync(int id, string userid)
        {
            var rows = await _dbconnection.ExecuteAsync(
                "sp_Semester_Delete",
                new
                {
                    Id = id,
                    UserId = userid
                },
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

  
        public async Task<bool> SemExist(int courseId, int orderNumber)
        {
            var count = await _dbconnection.ExecuteScalarAsync<int>(
                "sp_Semester_Exists",
                new
                {
                    CourseId = courseId,
                    OrderNumber = orderNumber
                },
                commandType: CommandType.StoredProcedure
            );

            return count > 0;
        }
    }
}