using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using Dapper;
using System.Data;

public class ClassRepository : IClassesRepository
{
    private readonly IDbConnection _connection;

    public ClassRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    // ✅ CREATE 
    public async Task<int> CreateAsync(Classes model)
    {
        var sql = @"INSERT INTO Classes (Name, DepartmentId, CourseId, Year, Semester)
                    VALUES (@Name, @DepartmentId, @CourseId, @Year, @Semester);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

        return await _connection.ExecuteScalarAsync<int>(sql, model);
    }

    // ✅ GET ALL
    public async Task<IEnumerable<Classes>> GetAllAsync()
    {
        var sql = "SELECT * FROM Classes WHERE IsActive = 1";
        return await _connection.QueryAsync<Classes>(sql);
    }

    // ✅ GET BY ID
    public async Task<Classes> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Classes WHERE Id = @id AND IsActive = 1";
        return await _connection.QueryFirstOrDefaultAsync<Classes>(sql, new { id });
    }

    // ✅ UPDATE
    public async Task UpdateAsync(int id, Classes model)
    {
        var sql = @"UPDATE Classes 
                    SET Name = @Name,
                        DepartmentId = @DepartmentId,
                        CourseId = @CourseId,
                        Year = @Year,
                        Semester = @Semester
                    WHERE Id = @id";

        await _connection.ExecuteAsync(sql, new
        {
            id,
            model.Name,
            model.DepartmentId,
            model.CourseId,
            model.Year,
            model.Semester
        });
    }

    // ✅ SOFT DELETE
    public async Task DeleteAsync(int id)
    {
        var sql = "UPDATE Classes SET IsActive = 0 WHERE Id = @id";
        await _connection.ExecuteAsync(sql, new { id });
    }

    // ✅ CHECK DUPLICATE
    public async Task<bool> ExistsAsync(int courseId, int year, int semester)
    {
        var sql = @"SELECT COUNT(1) 
                    FROM Classes 
                    WHERE CourseId = @courseId 
                    AND Year = @year 
                    AND Semester = @semester";

        var count = await _connection.ExecuteScalarAsync<int>(sql, new { courseId, year, semester });
        return count > 0;
    }
}