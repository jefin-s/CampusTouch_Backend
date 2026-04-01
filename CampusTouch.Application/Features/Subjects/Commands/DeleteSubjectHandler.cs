using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;

public class DeleteSubjectHandler : IRequestHandler<DeleteSubjectCommand, bool>
{
    private readonly ISubjectRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteSubjectHandler(
        ISubjectRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        // 1. Authorization
        if (!_currentUserService.IsAdmin)
            throw new UnauthorizedException("Only Admin can delete subject");

        // 2. Check existence
        var subject = await _repository.GetByIdAsync(request.Id);

        if (subject == null || subject.IsDeleted)
            throw new NotFoundException("Subject not found");

        // 3. Soft delete with audit
        return await _repository.DeleteAsync(request.Id, userId);
    }
}