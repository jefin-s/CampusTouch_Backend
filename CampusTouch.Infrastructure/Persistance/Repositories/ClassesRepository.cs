//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using Dapper;
//using System.Data;

//public class ClassRepository : IClassesRepository
//{
//    private readonly IDbConnection _connection;

//    public ClassRepository(IDbConnection connection)
//    {
//        _connection = connection;
//    }

//    public async Task<int> CreateAsync(Classes model, IDbTransaction transaction)
//    {
//        return await _connection.ExecuteScalarAsync<int>(
//            "sp_Class_Create",
//            model,
//            transaction: transaction,
//            commandType: CommandType.StoredProcedure
//        );
//    }

//    // ✅ GET ALL
//    public async Task<IEnumerable<Classes>> GetAllAsync(IDbTransaction transaction)
//    {
//        return await _connection.QueryAsync<Classes>(
//            "sp_Class_GetAll",
//            transaction: transaction,
//            commandType: CommandType.StoredProcedure
//        );
//    }

//    // ✅ GET BY ID
//    public async Task<Classes> GetByIdAsync(int id, IDbTransaction transaction)
//    {
//        return await _connection.QueryFirstOrDefaultAsync<Classes>(
//            "sp_Class_GetById",
//            new { Id = id },
//            transaction: transaction, 
//            commandType: CommandType.StoredProcedure
//        );
//    }

//    // ✅ UPDATE
//    public async Task UpdateAsync(int id, Classes model, IDbTransaction transaction)
//    {
//        await _connection.ExecuteAsync(
//            "sp_Class_Update",
//            new
//            {
//                Id = id,
//                model.Name,
//                model.DepartmentId,
//                model.CourseId,
//                model.Year,
//                model.Semester
//            },
//            transaction: transaction, // ✅ important
//            commandType: CommandType.StoredProcedure
//        );
//    }

//    // ✅ SOFT DELETE
//    public async Task DeleteAsync(int id, IDbTransaction transaction)
//    {
//        await _connection.ExecuteAsync(
//            "sp_Class_Delete",
//            new { Id = id },
//            transaction: transaction, // ✅ important
//            commandType: CommandType.StoredProcedure
//        );
//    }

//    // ✅ CHECK DUPLICATE
//    public async Task<bool> ExistsAsync(int courseId, int year, int semester,IDbTransaction transaction)
//    {
//        var count = await _connection.ExecuteScalarAsync<int>(
//            "sp_Class_Exists",
//            new { courseId, year, semester },
//            transaction: transaction,
//            commandType: CommandType.StoredProcedure
//        );

//        return count > 0;
//    }
//}

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
        return await _connection.ExecuteScalarAsync<int>(
            "sp_Class_Create",
            new
            {
                model.Name,
                model.DepartmentId,
                model.CourseId,
                model.Year,
                model.Semester
            },
            commandType: CommandType.StoredProcedure
        );
    }

    // ✅ GET ALL
    public async Task<IEnumerable<Classes>> GetAllAsync()
    {
        return await _connection.QueryAsync<Classes>(
            "sp_Class_GetAll",
      
            commandType: CommandType.StoredProcedure
        );
    }

    // ✅ GET BY ID
    public async Task<Classes> GetByIdAsync(int id)
    {
        return await _connection.QueryFirstOrDefaultAsync<Classes>(
            "sp_Class_GetById",
            new { Id = id },
           
            commandType: CommandType.StoredProcedure
        );
    }

    // ✅ UPDATE
    public async Task UpdateAsync(int id, Classes model)
    {
        await _connection.ExecuteAsync(
            "sp_Class_Update",
            new
            {
                Id = id,
                model.Name,
                model.DepartmentId,
                model.CourseId,
                model.Year,
                model.Semester
            },
          
            commandType: CommandType.StoredProcedure
        );
    }

    // ✅ DELETE
    public async Task DeleteAsync(int id)
    {
        await _connection.ExecuteAsync(
            "sp_Class_Delete",
            new { Id = id },
          
            commandType: CommandType.StoredProcedure
        );  
    }

    // ✅ EXISTS
    public async Task<bool> ExistsAsync(int courseId, int year, int semester)
    {
        var count = await _connection.ExecuteScalarAsync<int>(
            "sp_Class_Exists",
            new { courseId, year, semester },
           
            commandType: CommandType.StoredProcedure
        );

        return count > 0;
    }
}