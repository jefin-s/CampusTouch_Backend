//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;

//public class RemoveSubjectFromStaffHandler : IRequestHandler<RemoveSubjectFromStaffCommand, bool>
//{
//    private readonly IStaffRepository _staffRepository;
//    private readonly IStaffSubjectRepository _staffSubjectRepository;

//    public RemoveSubjectFromStaffHandler(
//        IStaffRepository staffRepository,
//        IStaffSubjectRepository staffSubjectRepository)
//    {
//        _staffRepository = staffRepository;
//        _staffSubjectRepository = staffSubjectRepository;
//    }

//    public async Task<bool> Handle(RemoveSubjectFromStaffCommand request, CancellationToken cancellationToken)
//    {
//        var staff = await _staffRepository.GetStaffById(request.StaffId);
//        if (staff == null)
//            throw new NotFoundException("Staff not found");

//        var exists = await _staffSubjectRepository.Exists(request.StaffId, request.SubjectId);
//        if (!exists)
//            throw new NotFoundException("Subject not assigned to this staff");

//        var result = await _staffSubjectRepository.RemoveAsync(request.StaffId, request.SubjectId);

//        return result > 0;
//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public class RemoveSubjectFromStaffHandler : IRequestHandler<RemoveSubjectFromStaffCommand, bool>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IStaffSubjectRepository _staffSubjectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RemoveSubjectFromStaffHandler> _logger;

    public RemoveSubjectFromStaffHandler(
        IStaffRepository staffRepository,
        IStaffSubjectRepository staffSubjectRepository,
        ICurrentUserService currentUserService,
        ILogger<RemoveSubjectFromStaffHandler> logger)
    {
        _staffRepository = staffRepository;
        _staffSubjectRepository = staffSubjectRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(RemoveSubjectFromStaffCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        // ✅ Attempt log
        _logger.LogInformation(
            "User {UserId} attempting to remove Subject {SubjectId} from Staff {StaffId}",
            userId, request.SubjectId, request.StaffId);

        // 🔐 Authorization
        if (!_currentUserService.IsAdmin)
        {
            _logger.LogWarning(
                "Unauthorized remove subject attempt by User {UserId} for Staff {StaffId}",
                userId, request.StaffId);

            throw new UnauthorizedException("Only admin can remove subject from staff");
        }

        // 🔍 Validate staff
        var staff = await _staffRepository.GetStaffById(request.StaffId);

        if (staff == null)
        {
            _logger.LogWarning(
                "Remove subject failed: Staff {StaffId} not found (User {UserId})",
                request.StaffId, userId);

            throw new NotFoundException("Staff not found");
        }

        // 🔍 Check mapping exists
        var exists = await _staffSubjectRepository.Exists(request.StaffId, request.SubjectId);

        if (!exists)
        {
            _logger.LogWarning(
                "Remove subject failed: Subject {SubjectId} not assigned to Staff {StaffId} (User {UserId})",
                request.SubjectId, request.StaffId, userId);

            throw new NotFoundException("Subject not assigned to this staff");
        }

        var result = await _staffSubjectRepository.RemoveAsync(request.StaffId, request.SubjectId);

        if (result > 0)
        {
            // ✅ Audit log
            _logger.LogInformation(
                "User {UserId} removed Subject {SubjectId} from Staff {StaffId} successfully",
                userId, request.SubjectId, request.StaffId);
        }
        else
        {
            // ❌ Unexpected failure
            _logger.LogError(
                "Failed to remove Subject {SubjectId} from Staff {StaffId} by User {UserId}",
                request.SubjectId, request.StaffId, userId);
        }

        return result > 0;
    }
}