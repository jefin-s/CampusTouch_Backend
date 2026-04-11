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
    public  class AttendenceRepository:IAttendenceRepository
    {

        private readonly IDbConnection _dbconnection;
        public AttendenceRepository(IDbConnection dbConnection)
        {
            _dbconnection = dbConnection;
        }
        public async Task<int> CreateAttendanceAsync(Attendence attendance, IDbTransaction transaction)
        {
            var sql = @"
            INSERT INTO Attendance (AttendanceDate, ClassId, SubjectId, StaffId)
            VALUES (@AttendanceDate, @ClassId, @SubjectId, @StaffId);

            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

            return await _dbconnection.ExecuteScalarAsync<int>(
                sql,
                attendance,
                transaction
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
            var sql = @"
            SELECT COUNT(1)
            FROM Attendance
            WHERE AttendanceDate = @date
              AND ClassId = @classId
              AND SubjectId = @subjectId
              AND IsDeleted = 0;
        ";

            var count = await _dbconnection.ExecuteScalarAsync<int>(
                sql,
                new { date, classId, subjectId }
            );

            return count > 0;
        }
        }
}
