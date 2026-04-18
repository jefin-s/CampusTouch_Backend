//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;

//public class UpdateSubjectHandler : IRequestHandler<UpdateSubjectCommand, bool>
//{
//    private readonly ISubjectRepository _repository;
//    private readonly ICurrentUserService _currentUserService;

//    public UpdateSubjectHandler(
//        ISubjectRepository repository,
//        ICurrentUserService currentUserService)
//    {
//        _repository = repository;
//        _currentUserService = currentUserService;
//    }

//    public async Task<bool> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
//    {

//        // 1. Authorization
//        if (!_currentUserService.IsAdmin)
//            throw new UnauthorizedException("Only Admin can update subject");
//        var userId = _currentUserService.UserId;

//        // 2. Get existing
//        var existing = await _repository.GetByIdAsync(request.Id);

//        if (existing == null || existing.IsDeleted)
//            throw new NotFoundException("Subject not found");

//        // 3. Duplicate check (IMPORTANT 🔥)
//        var exists = await _repository.Exist(request.SemesterId, request.Code);

//        if (exists && existing.Id != request.Id)
//            throw new BuisnessRuleException("Subject already exists in this semester");

//        // 4. Update existing entity
//        existing.Name = request.Name;
//        existing.Code = request.Code;
//        existing.Credits = request.Credits;
//        existing.SemesterId = request.SemesterId;

//        // 5. Audit
//        existing.UpdatedAt = DateTime.UtcNow;
//        existing.updatedby = userId;

//        // 6. Save
//        return await _repository.UpdateAsync(existing);
//    }
//}
using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public class UpdateSubjectHandler : IRequestHandler<UpdateSubjectCommand, bool>
{
    private readonly ISubjectRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateSubjectHandler> _logger;

    public UpdateSubjectHandler(
        ISubjectRepository repository,
        ICurrentUserService currentUserService,
        ILogger<UpdateSubjectHandler> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var code = request.Code.Trim().ToUpper();

        // ✅ Attempt log
        _logger.LogInformation(
            "User {UserId} attempting to update subject {SubjectId}",
            userId, request.Id);

        // 🔐 Authorization
        if (!_currentUserService.IsAdmin)
        {
            _logger.LogWarning(
                "Unauthorized update attempt by User {UserId} for Subject {SubjectId}",
                userId, request.Id);

            throw new UnauthorizedException("Only admin can update subject");
        }

        // 🔍 Get existing
        var existing = await _repository.GetByIdAsync(request.Id);

        if (existing == null || existing.IsDeleted)
        {
            _logger.LogWarning(
                "Update failed: Subject {SubjectId} not found (User {UserId})",
                request.Id, userId);

            throw new NotFoundException("Subject not found");
        }

        // 🔁 Duplicate check
        var exists = await _repository.Exist(request.SemesterId, code);

        if (exists && existing.Id != request.Id)
        {
            _logger.LogWarning(
                "Duplicate subject update attempt {Code} in Semester {SemesterId} by User {UserId}",
                code, request.SemesterId, userId);

            throw new BuisnessRuleException("Subject already exists");
        }

        // 📝 Capture old values (audit)
        var oldCode = existing.Code;
        var oldName = existing.Name;

        // 📝 Update
        existing.Name = request.Name;
        existing.Code = code;
        existing.Credits = request.Credits;
        existing.SemesterId = request.SemesterId;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.updatedby = userId;

        var result = await _repository.UpdateAsync(existing);

        if (result)
        {
            // ✅ Audit log (VERY IMPORTANT)
            _logger.LogInformation(
                "User {UserId} updated subject {SubjectId} successfully. Code: {OldCode} → {NewCode}, Name: {OldName} → {NewName}",
                userId, request.Id, oldCode, code, oldName, request.Name);
        }
        else
        {
            // ❌ Unexpected failure
            _logger.LogError(
                "Failed to update subject {SubjectId} by User {UserId}",
                request.Id, userId);
        }

        return result;
    }
}