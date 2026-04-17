using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using Dapper;
using System.Data;

namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public class DepartementRepository : IDepartementRepository
    {
        private readonly IDbConnection _connection;

        public DepartementRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        // ✅ CREATE
        public async Task<int> CreateAsync(Departement department)
        {
            return await _connection.ExecuteScalarAsync<int>(
                "sp_Department_Create",
                new
                {
                    department.Name,
                    department.Description,
                    department.CreatedBy
                },
                commandType: CommandType.StoredProcedure
            );
        }

        // ✅ GET BY ID
        public async Task<Departement> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<Departement>(
                "sp_Department_GetById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        // ✅ GET ALL
        public async Task<IEnumerable<Departement>> GetAllAsync()
        {
            return await _connection.QueryAsync<Departement>(
                "sp_Department_GetAll",
                commandType: CommandType.StoredProcedure
            );
        }

        // ✅ UPDATE
        public async Task<int> UpdateAsync(Departement department)
        {
            return await _connection.ExecuteAsync(
                "sp_Department_Update",
                new
                {
                    department.Id,
                    department.Name,
                    department.Description
                },
                commandType: CommandType.StoredProcedure
            );
        }

        // ✅ DELETE (Soft Delete)
        public async Task<int> DeleteAsync(int id)
        {
            return await _connection.ExecuteAsync(
                "sp_Department_Delete",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        // ✅ EXISTS
        public async Task<bool> DepartemnetExist(string name)
        {
            var count = await _connection.ExecuteScalarAsync<int>(
                "sp_Department_Exists",
                new { Name = name },
                commandType: CommandType.StoredProcedure
            );

            return count > 0;
        }
    }
}