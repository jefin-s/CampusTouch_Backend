using CampusTouch.Application.Features.Attendence.DTO;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using Dapper;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public  class AttendenceRepository:IAttendenceRepository
    {

        private readonly IDbConnection _dbconnection;
        public AttendenceRepository(IDbConnection dbConnection)
        {
            _dbconnection = dbConnection;
        }
        public async Task<int> CreateAttendanceAsync(Attendence attendance, IDbTransaction transaction)
        {
            return await _dbconnection.ExecuteScalarAsync<int>(
                "SP_CreateAttendance",
                new
                {
                    AttendanceDate = attendance.AttendanceDate,
                    ClassId = attendance.ClassId,
                    SubjectId = attendance.SubjectId,
                    StaffId = attendance.StaffId
                },
                transaction,
                commandType: CommandType.StoredProcedure
            );
        }

        // 🔹 Create AttendanceDetails (Child - Bulk Insert)
        public async Task CreateAttendanceDetailsAsync(List<AttendenceDetails> details, IDbTransaction transaction)
        {
            var sql = @"
            INSERT INTO AttendenceDetails (AttendanceId, StudentId, Status)
            VALUES (@AttendanceId, @StudentId, @Status);
        ";

            await _dbconnection.ExecuteAsync(
                sql,
                details,
                transaction
            );
        }

        // 🔹 Check Duplicate Attendance
        public async Task<bool> ExistsAsync(DateTime date, int classId, int subjectId)
        {
            return await _dbconnection.ExecuteScalarAsync<bool>(
                "SP_AttendanceExists",
                new
                {
                    AttendanceDate = date,
                    ClassId = classId,
                    SubjectId = subjectId
                },
                commandType: CommandType.StoredProcedure
            );
        }


        public async Task<List<AttendenceViewDto>> GetAttendanceByClassAsync(int classId, DateTime date)
        {
            var result = await _dbconnection.QueryAsync<AttendenceViewDto>(
                "SP_GetAttendenceByClass",
                new
                {
                    classid = classId,
                    date = date
                },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<List<AttendenceViewDto>> GetAttendenceByStudentId(int studentId)
        {
            var result = await _dbconnection.QueryAsync<AttendenceViewDto>(
                "SP_GetAttendanceByStudentId",
                new { studentId },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<int?> GetStudentIdByUserIdAsync(string userId)
        {
            var sql = @"select Id from students where userid=@userId";
            return await _dbconnection.ExecuteScalarAsync<int?>(sql, new { userId });
        }
    }
}
