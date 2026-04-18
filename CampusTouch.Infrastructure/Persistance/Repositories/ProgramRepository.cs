using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using Dapper;
using System.Data;

namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public class ProgramRepository : IProgramRepository
    {
        private readonly IDbConnection _dbconnection;

        public ProgramRepository(IDbConnection dbConnection)
        {
            _dbconnection = dbConnection;
        }

        // ✅ CREATE
        public async Task<int> CreateAsync(AcademicProgram course)
        {
            return await _dbconnection.ExecuteScalarAsync<int>(
                "sp_Program_Create",
                new
                {
                    course.Name,
                    course.Level,
                    course.Duration,
                    course.DepartmentId,
                    course.CreatedBy
                },
                commandType: CommandType.StoredProcedure
            );
        }

        // ✅ GET ALL
        public async Task<IEnumerable<AcademicProgram>> GetAllAsync()
        {
            return await _dbconnection.QueryAsync<AcademicProgram>(
                "sp_Program_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        // ✅ GET BY ID
        public async Task<AcademicProgram?> GetByIdAsync(int id)
        {
            return await _dbconnection.QueryFirstOrDefaultAsync<AcademicProgram>(
                "sp_Program_GetById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        // ✅ UPDATE
        public async Task<int> UpdateAsync(AcademicProgram course)
        {
            return await _dbconnection.ExecuteAsync(
                "sp_Program_Update",
                new
                {
                    course.Id,
                    course.Name,
                    course.Level,
                    course.Duration,
                    course.DepartmentId,
                    course.UpdatedBy
                },
                commandType: CommandType.StoredProcedure
            );
        }

        // ✅ DELETE
        public async Task<int> DeleteAsync(int id)
        {
            return await _dbconnection.ExecuteAsync(
                "sp_Program_Delete",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        // ✅ EXISTS
        public async Task<bool> ProgramIsExist(string course, int deptId)
        {
            var count = await _dbconnection.ExecuteScalarAsync<int>(
                "sp_Program_Exists",
                new
                {
                    Name = course,
                    DepartmentId = deptId
                },
                commandType: CommandType.StoredProcedure
            );

            return count > 0;
        }
    }
}