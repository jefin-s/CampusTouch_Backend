using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using Dapper;
using System.Data;

namespace CampusTouch.Infrastructure.Persistence.Repositories
{
    public class StaffSubjectRepository : IStaffSubjectRepository
    {
        private readonly IDbConnection _dbConnection;

        public StaffSubjectRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // 🔥 1. Add
        public async Task<int> AddAsync(StaffSubject entity)
        {
            var sql = @"
                INSERT INTO StaffSubject (StaffId, SubjectId, CreatedAt)
                VALUES (@StaffId, @SubjectId, @CreatedAt)";

            return await _dbConnection.ExecuteAsync(sql, entity);
        }

        // 🔥 2. Check Exists
        public async Task<bool> Exists(int staffId, int subjectId)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM StaffSubject
                WHERE StaffId = @StaffId AND SubjectId = @SubjectId";

            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                StaffId = staffId,
                SubjectId = subjectId
            });

            return count > 0;
        }

        // 🔥 3. Get Subjects by Staff
        public async Task<List<int>> GetSubjectsByStaffId(int staffId)
        {
            var sql = @"
                SELECT SubjectId
                FROM StaffSubject
                WHERE StaffId = @StaffId";

            var result = await _dbConnection.QueryAsync<int>(sql, new
            {
                StaffId = staffId
            });

            return result.ToList();
        }

        // 🔥 4. Remove One
        public async Task<int> RemoveAsync(int staffId, int subjectId)
        {
            var sql = @"
                DELETE FROM StaffSubject
                WHERE StaffId = @StaffId AND SubjectId = @SubjectId";

            return await _dbConnection.ExecuteAsync(sql, new
            {
                StaffId = staffId,
                SubjectId = subjectId
            });
        }

        // 🔥 5. Remove All (IMPORTANT)
        public async Task<int> RemoveAllByStaffId(int staffId)
        {
            var sql = @"
                DELETE FROM StaffSubject
                WHERE StaffId = @StaffId";

            return await _dbConnection.ExecuteAsync(sql, new
            {
                StaffId = staffId
            });
        }
    }
}