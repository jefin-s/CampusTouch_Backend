using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System.Data;

public class CreateAttendanceHandler : IRequestHandler<CreateAttendanceCommand, int>
{
    private readonly IAttendenceRepository _repo;
    private readonly IDbConnection _connection;
    private readonly ICurrentUserService _currentUser;

    public CreateAttendanceHandler(
        IAttendenceRepository repo,
        IDbConnection connection,
        ICurrentUserService currentUser)
    {
        _repo = repo;
        _connection = connection;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
    {
        // 🔐 Authorization (also fix message)
        if (!_currentUser.IsAdmin)
            throw new UnauthorizedAccessException("Only staff can mark attendance");

        // 🔥 Duplicate check
        if (await _repo.ExistsAsync(request.Date, request.ClassId, request.SubjectId))
            throw new Exception("Attendance already marked");

        // ✅ OPEN CONNECTION (IMPORTANT)
        if (_connection.State == ConnectionState.Closed)
            _connection.Open();

        using var transaction = _connection.BeginTransaction();

        try
        {
            // 🧱 Create master
            var attendance = new Attendence
            {
                AttendanceDate = request.Date,
                ClassId = request.ClassId,
                SubjectId = request.SubjectId,
                StaffId= _currentUser.UserId,

            };

            var attendanceId = await _repo.CreateAttendanceAsync(attendance, transaction);

            // 🧱 Create details
            var details = request.Students.Select(s => new AttendenceDetails
            {
                AttendanceId = attendanceId,
                StudentId = s.StudentId,
                Status = s.Status
            }).ToList();

            await _repo.CreateAttendanceDetailsAsync(details, transaction);

            // ✅ COMMIT
            transaction.Commit();

            return attendanceId;
        }
        catch
        {
            // ❌ ROLLBACK
            transaction.Rollback();
            throw;
        }
    }
}
