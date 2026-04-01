using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public class SemseterRepository:ISemsterRepository
    {
        private readonly IDbConnection _dbconnection;
        public SemseterRepository(IDbConnection dbConnection)
        {
            _dbconnection = dbConnection;
        }


        public async Task<int> CreateAsync(Semesters semester)
        {
            var query = @"
                INSERT INTO Semester (Name, OrderNumber, CourseId, IsActive, CreatedA)
                VALUES (@Name, @OrderNumber, @CourseId, 1, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return  await _dbconnection.ExecuteScalarAsync<int>(query, semester);
        }

        public async Task<IEnumerable<Semesters>> GetAllAsync()
        {
            var query = "SELECT * FROM Semester where isactive=1";
            return await _dbconnection.QueryAsync<Semesters>(query);

        }
        public async Task<Semesters?> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Semester WHERE Id = @Id AND IsActive=1";
            return await _dbconnection.QueryFirstOrDefaultAsync<Semesters>(query, new { Id = id });
        }
        public async Task<bool> UpdateAsync(Semesters semester)
        {
                       var query = @"
                UPDATE Semesters
                SET Name = @Name, OrderNumber = @OrderNumber, CourseId = @CourseId, UpdatedAt = GETDATE()
                WHERE Id = @Id AND IsActive=1
            ";
            var rowsAffected = await _dbconnection.ExecuteAsync(query, semester);
            return rowsAffected > 0;

        }

        public async Task<bool> DeleteAsync(int id,string userid)
        {
            var query = @"UPDATE Semester 
                  SET IsDeleted = 1,
                      DeletedAt = GETUTCDATE(),
                      DeletedBy = @UserId
                  WHERE Id = @Id";
            var rowsAffected = await _dbconnection.ExecuteAsync(query, new { Id = id,userid=userid });
            return rowsAffected > 0;
        }

        public async Task<bool> SemExist(int courseId, int orderNumber)
        {
            var query = @"SELECT COUNT(1)
                  FROM Semester
                  WHERE CourseId = @CourseId
                  AND OrderNumber = @OrderNumber
                  AND IsDeleted = 0";

            var count = await _dbconnection.ExecuteScalarAsync<int>(query, new
            {
                CourseId = courseId,
                OrderNumber = orderNumber
            });

            return count > 0;
        }
    }
}
