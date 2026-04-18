//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;

//public class DeleteSubjectHandler : IRequestHandler<DeleteSubjectCommand, bool>
//{
//    private readonly ISubjectRepository _repository;
//    private readonly ICurrentUserService _currentUserService;

//    public DeleteSubjectHandler(
//        ISubjectRepository repository,
//        ICurrentUserService currentUserService)
//    {
//        _repository = repository;
//        _currentUserService = currentUserService;
//    }

//    public async Task<bool> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
//    {

//        // 1. Authorization
//        if (!_currentUserService.IsAdmin)
//            throw new UnauthorizedException("Only Admin can delete subject");
//        var userId = _currentUserService.UserId;

//        // 2. Check existence
//        var subject = await _repository.GetByIdAsync(request.Id);

//        if (subject == null || subject.IsDeleted)
//            throw new NotFoundException("Subject not found");

//        // 3. Soft delete with audit
//        return await _repository.DeleteAsync(request.Id, userId);
//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public class DeleteSubjectHandler : IRequestHandler<DeleteSubjectCommand, bool>
{
    private readonly ISubjectRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteSubjectHandler> _logger;

    public DeleteSubjectHandler(
        ISubjectRepository repository,
        ICurrentUserService currentUserService,
        ILogger<DeleteSubjectHandler> logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        // ✅ Attempt log
        _logger.LogInformation(
            "User {UserId} attempting to delete subject {SubjectId}",
            userId, request.Id);

        // 🔐 Authorization
        if (!_currentUserService.IsAdmin)
        {
            _logger.LogWarning(
                "Unauthorized delete attempt by User {UserId} for Subject {SubjectId}",
                userId, request.Id);

            throw new UnauthorizedException("Only admin can delete subject");
        }

        // 🔍 Check existence
        var subject = await _repository.GetByIdAsync(request.Id);

        if (subject == null || subject.IsDeleted)
        {
            _logger.LogWarning(
                "Delete failed: Subject {SubjectId} not found (User {UserId})",
                request.Id, userId);

            throw new NotFoundException("Subject not found");
        }

        var result = await _repository.DeleteAsync(request.Id, userId);

        if (result)
        {
            // ✅ Audit log (VERY IMPORTANT)
            _logger.LogInformation(
                "User {UserId} deleted subject {SubjectId} ({Code}) successfully",
                userId, request.Id, subject.Code);
        }
        else
        {
            // ❌ Unexpected failure
            _logger.LogError(
                "Failed to delete subject {SubjectId} by User {UserId}",
                request.Id, userId);
        }

        return result;
    }
}