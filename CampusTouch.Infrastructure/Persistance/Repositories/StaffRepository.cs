using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly IDbConnection _dbconnection;

        public StaffRepository(IDbConnection dbConnection)
        {
            _dbconnection = dbConnection;
        }

        public async Task<int> CreateNewStaff(Staff newStaff)
        {
            var sql = @"INSERT INTO Staff (UserId, FirstName, LastName, Email, PhoneNumber, DepartmentId, Designation, EmployeeCode, JoiningDate, IsActive, CreatedAt, CreatedBy)
                         VALUES (@UserId, @FirstName, @LastName, @Email, @PhoneNumber, @DepartmentId, @Designation, @EmployeeCode, @JoiningDate, @IsActive, @CreatedAt, @CreatedBy);
                         SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _dbconnection.QuerySingleAsync<int>(sql, newStaff);
        }


        public async Task<IEnumerable<Staff>> GetAllStaffs()
        {
            var query = "SELECT * FROM Staff where isactive=1";
            return await _dbconnection.QueryAsync<Staff>(query);
        }

        public async Task<Staff?> GetStaffById(int id)
        {
            var query = "SELECT * FROM Staff WHERE Id = @Id";
            return await _dbconnection.QueryFirstOrDefaultAsync<Staff>(query, new { Id = id });
        }

        public async Task<int> UpdateStaff(Staff staff)
        {
            var sql = @"UPDATE Staff 
                        SET FirstName = @FirstName, LastName = @LastName, Email = @Email, PhoneNumber = @PhoneNumber, 
                            DepartmentId = @DepartmentId, Designation = @Designation, EmployeeCode = @EmployeeCode, 
                            JoiningDate = @JoiningDate, UpdatedAt = @UpdatedAt
                        WHERE Id = @Id";
            return await _dbconnection.ExecuteAsync(sql, staff);
        }

        public async Task<int> DeactivateStaff(int id)
        {
            var sql = @"UPDATE Staff 
                        SET IsActive = 0, UpdatedAt = @UpdatedAt
                        WHERE Id = @Id";
            return await _dbconnection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
        }

        public async Task<Staff?> GetByUserId(string userId)
        {
            var query = "SELECT * FROM Staff WHERE UserId = @UserId AND IsActive = 1";
            return await _dbconnection.QueryFirstOrDefaultAsync<Staff>(query, new { UserId = userId });

        }
    }
}
