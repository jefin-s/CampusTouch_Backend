using CampusTouch.Application.Features.Attendence.DTO;
using CampusTouch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Interfaces
{
    public interface IAttendenceRepository
    {
        Task<int> CreateAttendanceAsync(Attendence attendance, IDbTransaction transaction);

        Task CreateAttendanceDetailsAsync(List<AttendenceDetails> details, IDbTransaction transaction);

        Task<bool> ExistsAsync(DateTime date, int classId, int subjectId);
        Task<List<AttendenceViewDto>> GetAttendanceByClassAsync(int classId, DateTime date);
    }
}
