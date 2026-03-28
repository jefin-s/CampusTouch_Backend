using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using Dapper;

using System.Data;


namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public class DepartementRepository:IDepartementRepository
    {
        private readonly IDbConnection _connection;
        public DepartementRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> CreateAsync(Departement department)
        {
            var sql = @"INSERT INTO Departments (Name, Description, IsActive, CreatedAt)
                VALUES (@Name, @Description, @IsActive, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _connection.QuerySingleAsync<int>(sql, department);
        }


        public async Task<Departement> GetByIdAsync(int id)
        {
            var sql = "select *from Departments where Id=@id";
            return await _connection.QueryFirstOrDefaultAsync<Departement>(sql, new { id});

        }

        public async Task<IEnumerable<Departement>> GetAllAsync()
        {
            var sql = "select *from   Departments  where isactive=1 order by id";
            return  await _connection.QueryAsync<Departement>(sql);
        }


        public async  Task<int> UpdateAsync(Departement department)
        {
            var sql= @"udpate departments set Name=@Name ,Description = @Description,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @Id";
                return await _connection.ExecuteAsync(sql, department);
        
        }
        public async Task<int> DeleteAsync(int id)
        {
            var sql = @"update students set isactive=0 ,UpdatedAt = GETUTCDATE() where Id=@id";
            return await _connection.ExecuteAsync(sql, id);
        }
    }
}
