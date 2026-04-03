using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using CloudinaryDotNet;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public class ProgramRepository:IProgramRepository
    {

        private readonly IDbConnection _dbconnection;
        public ProgramRepository(IDbConnection dbConnection)
        {
            _dbconnection = dbConnection;

        }

        public async Task<int> CreateAsync(AcademicProgram course)
        {
            var sql = @"INSERT INTO Courses (Name, Level, Duration, DepartmentId, CreatedBy, CreatedAt)
                    VALUES (@Name, @Level, @Duration, @DepartmentId, @CreatedBy, @CreatedAt);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _dbconnection.ExecuteScalarAsync<int>(sql, course);
        }

        public async Task<IEnumerable<AcademicProgram>> GetAllAsync()
        {
            var sql = "SELECT * FROM Courses";
            return await _dbconnection.QueryAsync<AcademicProgram>(sql);
        }

        public async Task<AcademicProgram?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Courses WHERE Id = @Id";
            return await _dbconnection.QueryFirstOrDefaultAsync<AcademicProgram>(sql, new { Id = id });
        }

        public async Task<int> UpdateAsync(AcademicProgram course)
        {
            var sql = @"UPDATE Courses 
                    SET Name = @Name, Level = @Level, Duration = @Duration,
                        DepartmentId = @DepartmentId, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";

            return await _dbconnection.ExecuteAsync(sql, course);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = @"UPDATE Courses 
                SET IsActive = 0,
                    IsDeleted = 1,
                    DeletedBy = @UserId,
                    DeletedAt = @DeletedAt
                WHERE Id = @Id";

            return await _dbconnection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<bool> ProgramIsExist(string course, int deptId)
        {
            var sql = "SELECT COUNT(1) FROM Courses WHERE Name = @couse AND DepartmentId = @deptId AND IsActive = 1";
            var count = await _dbconnection.ExecuteScalarAsync<int>(sql, new { couse = course, deptId = deptId });
            return count > 0;
        }
    }
}
