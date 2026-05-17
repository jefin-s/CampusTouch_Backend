//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using MediatR;
//using System.Data;

//public class CreateAttendanceHandler : IRequestHandler<CreateAttendanceCommand, int>
//{
//    private readonly IAttendenceRepository _repo;
//    private readonly IDbConnection _connection;
//    private readonly ICurrentUserService _currentUser;

//    public CreateAttendanceHandler(
//        IAttendenceRepository repo,
//        IDbConnection connection,
//        ICurrentUserService currentUser)
//    {
//        _repo = repo;
//        _connection = connection;
//        _currentUser = currentUser;
//    }

//    public async Task<int> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
//    {
//        // 🔐 Authorization (also fix message)
//        if (!_currentUser.IsAdmin)
//            throw new UnauthorizedAccessException("Only staff can mark attendance");

//        // 🔥 Duplicate check
//        if (await _repo.ExistsAsync(request.Date, request.ClassId, request.SubjectId))
//            throw new Exception("Attendance already marked");

//        // ✅ OPEN CONNECTION (IMPORTANT)
//        if (_connection.State == ConnectionState.Closed)
//            _connection.Open();

//        using var transaction = _connection.BeginTransaction();

//        try
//        {
//            // 🧱 Create master
//            var attendance = new Attendence
//            {
//                AttendanceDate = request.Date,
//                ClassId = request.ClassId,
//                SubjectId = request.SubjectId,
//                StaffId= _currentUser.UserId,

//            };

//            var attendanceId = await _repo.CreateAttendanceAsync(attendance, transaction);

//            // 🧱 Create details
//            var details = request.Students.Select(s => new AttendenceDetails
//            {
//                AttendanceId = attendanceId,
//                StudentId = s.StudentId,
//                Status = s.Status
//            }).ToList();

//            await _repo.CreateAttendanceDetailsAsync(details, transaction);

//            // ✅ COMMIT
//            transaction.Commit();

//            return attendanceId;
//        }
//        catch
//        {
//            // ❌ ROLLBACK
//            transaction.Rollback();
//            throw;
//        }
//    }
//}

//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using MediatR;
//using Microsoft.Extensions.Logging;
//using System.Data;

//public class CreateAttendanceHandler : IRequestHandler<CreateAttendanceCommand, int>
//{
//    private readonly IAttendenceRepository _repo;
//    private readonly IDbConnection _connection;
//    private readonly ICurrentUserService _currentUser;
//    private readonly ILogger<CreateAttendanceHandler> _logger;

//    public CreateAttendanceHandler(
//        IAttendenceRepository repo,
//        IDbConnection connection,
//        ICurrentUserService currentUser,
//        ILogger<CreateAttendanceHandler> logger)
//    {
//        _repo = repo;
//        _connection = connection;
//        _currentUser = currentUser;
//        _logger = logger;
//    }

//    public async Task<int> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
//    {
//        var userId = _currentUser.UserId;

//        // ✅ Attempt log
//        _logger.LogInformation(
//            "User {UserId} attempting to mark attendance for Class {ClassId}, Subject {SubjectId} on {Date}",
//            userId, request.ClassId, request.SubjectId, request.Date);

//        // 🔐 Authorization
//        if (_currentUser.IsAdmin)
//        {
//            _logger.LogWarning(
//                "Unauthorized attendance attempt by User {UserId}",
//                userId);

//            throw new UnauthorizedAccessException("Admin Cannot marked attendence");
//        }

//        // 🔁 Duplicate check
//        if (await _repo.ExistsAsync(request.Date, request.ClassId, request.SubjectId,request.StudentId))
//        {
//            _logger.LogWarning(
//                "Duplicate attendance attempt for Class {ClassId}, Subject {SubjectId}, Date {Date} by User {UserId}",
//                request.ClassId, request.SubjectId, request.Date, userId);

//            throw new BuisnessRuleException("Attendance already marked");
//        }

//        if (_connection.State == ConnectionState.Closed)
//            _connection.Open();

//        using var transaction = _connection.BeginTransaction();

//        try
//        {
//            // 🧱 Create master
//            var attendance = new Attendence
//            {
//                AttendanceDate = request.Date,
//                ClassId = request.ClassId,
//                SubjectId = request.SubjectId,
//                StaffId = userId,
//                IsDeleted= false,
//            };

//            var attendanceId = await _repo.CreateAttendanceAsync(attendance, transaction);

//            // 🧱 Create details
//            var details = request.Students.Select(s => new AttendenceDetails
//            {
//                AttendanceId = attendanceId,
//                StudentId = s.StudentId,
//                Status = s.Status,
//                Remark = s.Remark,
//                MarkedAt = DateTime.UtcNow,
//                IsEdited= false
//            }).ToList();

//            await _repo.CreateAttendanceDetailsAsync(details, transaction);

//            // ✅ Commit
//            transaction.Commit();

//            // ✅ Audit log (VERY IMPORTANT)
//            _logger.LogInformation(
//                "User {UserId} marked attendance {AttendanceId} for {StudentCount} students (Class {ClassId}, Subject {SubjectId})",
//                userId, attendanceId, details.Count, request.ClassId, request.SubjectId);

//            return attendanceId;
//        }
//        catch (Exception ex)
//        {
//            transaction.Rollback();

//            // ❌ Error log (CRITICAL)
//            _logger.LogError(
//                ex,
//                "Error marking attendance for Class {ClassId}, Subject {SubjectId}, Date {Date} by User {UserId}",
//                request.ClassId, request.SubjectId, request.Date, userId);

//            throw;
//        }
//    }
//}   


//using CampusTouch.Application.Features.Attendence.DTO;
//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using MediatR;
//using Microsoft.Extensions.Logging;
//using System.Data;

//public class CreateAttendanceHandler : IRequestHandler<CreateAttendanceCommand, CreateAttendenceResponseDTO>
//{
//    private readonly IAttendenceRepository _repo;
//    private readonly IDbConnection _connection;
//    private readonly ICurrentUserService _currentUser;
//    private readonly ILogger<CreateAttendanceHandler> _logger;

//    public CreateAttendanceHandler(
//        IAttendenceRepository repo,
//        IDbConnection connection,
//        ICurrentUserService currentUser,
//        ILogger<CreateAttendanceHandler> logger)
//    {
//        _repo = repo;
//        _connection = connection;
//        _currentUser = currentUser;
//        _logger = logger;
//    }

//    public async Task<CreateAttendenceResponseDTO> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
//    {
//        var userId = _currentUser.UserId;

//        _logger.LogInformation(
//            "User {UserId} attempting to mark attendance for Class {ClassId}, Subject {SubjectId} on {Date}",
//            userId, request.ClassId, request.SubjectId, request.Date);

//        // Authorization
//        if (_currentUser.IsAdmin)
//        {
//            _logger.LogWarning(
//                "Unauthorized attendance attempt by User {UserId}",
//                userId);

//            throw new UnauthorizedAccessException("Admin cannot mark attendance");
//        }

//        if (_connection.State == ConnectionState.Closed)
//            _connection.Open();

//        using var transaction = _connection.BeginTransaction();

//        try
//        {
//            // Check attendance already exists
//            var attendanceId = await _repo.GetAttendanceIdAsync(
//                request.Date,
//                request.ClassId,
//                request.SubjectId,
//                transaction);

//            // Create attendance only if not exists
//            if (attendanceId == 0)
//            {
//                var attendance = new Attendence
//                {
//                    AttendanceDate = request.Date,
//                    ClassId = request.ClassId,
//                    SubjectId = request.SubjectId,
//                    StaffId = userId,
//                    IsDeleted = false
//                };

//                attendanceId = await _repo.CreateAttendanceAsync(
//                    attendance,
//                    transaction);
//            }

//            // Create attendance details
//            var details = request.Students.Select(s => new AttendenceDetails
//            {
//                AttendanceId = attendanceId,
//                StudentId = s.StudentId,
//                Status = s.Status,
//                Remark = s.Remark,
//                MarkedAt = DateTime.UtcNow,
//                IsEdited = false
//            }).ToList();

//            await _repo.CreateAttendanceDetailsAsync(details, transaction);

//            transaction.Commit();

//            _logger.LogInformation(
//                "Attendance {AttendanceId} marked successfully for {StudentCount} students",
//                attendanceId,
//                details.Count);

//            return new CreateAttendenceResponseDTO
//            {
//                Success = true,
//                AttendanceId = attendanceId,
//                Message = "Attendance marked successfully",
//                StudentCount = details.Count
//            };
//        }
//        catch (Exception ex)
//        {
//            transaction.Rollback();

//            _logger.LogError(
//                ex,
//                "Error while marking attendance");

//            throw;
//        }
//    }
//}

using CampusTouch.Application.Features.Attendence.DTO;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

public class CreateAttendanceHandler
    : IRequestHandler<CreateAttendanceCommand, CreateAttendenceResponseDTO>
{
    private readonly IAttendenceRepository _repo;
    private readonly IDbConnection _connection;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateAttendanceHandler> _logger;

    public CreateAttendanceHandler(
        IAttendenceRepository repo,
        IDbConnection connection,
        ICurrentUserService currentUser,
        ILogger<CreateAttendanceHandler> logger)
    {
        _repo = repo;
        _connection = connection;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<CreateAttendenceResponseDTO> Handle(
        CreateAttendanceCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        _logger.LogInformation(
            "User {UserId} attempting to mark attendance for Class {ClassId}, Subject {SubjectId} on {Date}",
            userId,
            request.ClassId,
            request.SubjectId,
            request.Date);

        // Authorization
        if (_currentUser.IsAdmin)
        {
            _logger.LogWarning(
                "Unauthorized attendance attempt by User {UserId}",
                userId);

            throw new UnauthorizedAccessException(
                "Admin cannot mark attendance");
        }

        if (_connection.State == ConnectionState.Closed)
            _connection.Open();

        using var transaction = _connection.BeginTransaction();

        try
        {
            // Check attendance already exists
            var attendanceId = await _repo.GetAttendanceIdAsync(
                request.Date,
                request.ClassId,
                request.SubjectId,
                transaction);

            // Create attendance only if not exists
            if (attendanceId == 0)
            {
                var attendance = new Attendence
                {
                    AttendanceDate = request.Date,
                    ClassId = request.ClassId,
                    SubjectId = request.SubjectId,
                    StaffId = userId,
                    IsDeleted = false
                };

                attendanceId = await _repo.CreateAttendanceAsync(
                    attendance,
                    transaction);
            }

            // Create attendance details
            var details = request.Students.Select(s => new AttendenceDetails
            {
                AttendanceId = attendanceId,
                StudentId = s.StudentId,
                Status = s.Status,
                Remark = s.Remark,
                MarkedAt = DateTime.UtcNow,
                IsEdited = false
            }).ToList();

            await _repo.CreateAttendanceDetailsAsync(
                details,
                transaction);

            transaction.Commit();

            _logger.LogInformation(
                "Attendance {AttendanceId} marked successfully for {StudentCount} students",
                attendanceId,
                details.Count);

            return new CreateAttendenceResponseDTO
            {
                Success = true,
                AttendanceId = attendanceId,
                Message = "Attendance marked successfully",
                StudentCount = details.Count,

                Students = request.Students
            };
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            _logger.LogError(
                ex,
                "Error while marking attendance");

            throw;
        }
    }
}