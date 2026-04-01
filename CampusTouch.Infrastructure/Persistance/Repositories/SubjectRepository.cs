using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public class SubjectRepository:ISubjectRepository
    {
        private readonly IDbConnection _dbConnection;
        public SubjectRepository(IDbConnection connection)
        {
            _dbConnection = connection;
        }

        public async Task<int> CreateAsync(Subject subject)
        {
            var query = @"
                INSERT INTO Subjects (Name, Code, Credits, SemesterId, IsActive, CreatedAt)
                VALUES (@Name, @Code, @Credits, @SemesterId, 1, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            
            return await _dbConnection.ExecuteScalarAsync<int>(query, subject);
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            var query = "SELECT * FROM Subjects WHERE IsActive = 1";

           
            return await _dbConnection.QueryAsync<Subject>(query);
        }

        public async Task<Subject?> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Subjects WHERE Id = @Id";

            return await _dbConnection.QueryFirstOrDefaultAsync<Subject>(query, new { Id = id });
        }

        public async Task<bool> UpdateAsync(Subject subject)
        {
            var query = @"
                UPDATE Subjects
                SET Name = @Name,
                    Code = @Code,
                    Credits = @Credits,
                    SemesterId = @SemesterId,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id
            ";

          
            var result = await _dbConnection.ExecuteAsync(query, subject);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id,string userid)
        {
            var query = @"
                UPDATE Subjects
                SET IsDeleted = 1,
                    deletedAt = GETDATE()
                    deletedby=@userid
                WHERE Id = @Id and isdeleted =0
            ";

          
            var result = await _dbConnection.ExecuteAsync(query, new { Id = id, UserId = userid });
            return result > 0;  
        }

        public async Task<bool> Exist(int semesterId, string code)
        {
            var query = @"SELECT COUNT(1)
                  FROM Subjects
                  WHERE SemesterId = @SemesterId
                  AND Code = @Code
                  AND IsDeleted = 0";

            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new
            {
                SemesterId = semesterId,
                Code = code
            });

            return count > 0;
        }
    }
}
