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


        public async Task<List<AttendenceViewDto>> GetAttendanceByClassAsync(int classId, DateTime date)
        {
            var sql = @"
SELECT 
    ad.StudentId, 
    (s.FirstName + ' ' + s.LastName) AS StudentName, 
    ad.Status 
FROM Attendance a
JOIN AttendenceDetails ad ON ad.AttendanceId = a.Id
JOIN Students s ON s.Id = ad.StudentId
WHERE a.ClassId = @classid
AND CAST(a.AttendanceDate AS DATE) = CAST(@date AS DATE)
AND a.IsDeleted = 0";
            var result = await _dbconnection.QueryAsync<AttendenceViewDto>(
              sql,
              new { classId, date });
            return result.ToList();

        }

        public async Task<List<AttendenceViewDto>> GetAttendenceByStudentId(int studentId)
        {
            var sql = @"
    SELECT 
        a.AttendanceDate AS Date,
        sub.Name AS SubjectName,
        ad.Status
    FROM Attendance a
    JOIN AttendenceDetails ad ON ad.AttendanceId = a.Id
    JOIN Subjects sub ON sub.Id = a.SubjectId
    WHERE ad.StudentId = @studentId
      AND a.IsDeleted = 0
    ORDER BY a.AttendanceDate DESC
    ";

            var result = await _dbconnection.QueryAsync<AttendenceViewDto>(
                sql,
                new { studentId });

            return result.ToList();
        }

        public async Task<int?> GetStudentIdByUserIdAsync(string userId)
        {
            var sql = @"select Id from students where userid=@userId";
            return await _dbconnection.ExecuteScalarAsync<int?>(sql, new { userId });
        }
    }
}
