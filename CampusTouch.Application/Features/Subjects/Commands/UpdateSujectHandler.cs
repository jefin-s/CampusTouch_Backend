using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;

public class UpdateSubjectHandler : IRequestHandler<UpdateSubjectCommand, bool>
{
    private readonly ISubjectRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateSubjectHandler(
        ISubjectRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {

        // 1. Authorization
        if (!_currentUserService.IsAdmin)
            throw new UnauthorizedException("Only Admin can update subject");
        var userId = _currentUserService.UserId;

        // 2. Get existing
        var existing = await _repository.GetByIdAsync(request.Id);

        if (existing == null || existing.IsDeleted)
            throw new NotFoundException("Subject not found");

        // 3. Duplicate check (IMPORTANT 🔥)
        var exists = await _repository.Exist(request.SemesterId, request.Code);

        if (exists && existing.Id != request.Id)
            throw new BuisnessRuleException("Subject already exists in this semester");

        // 4. Update existing entity
        existing.Name = request.Name;
        existing.Code = request.Code;
        existing.Credits = request.Credits;
        existing.SemesterId = request.SemesterId;
         
        // 5. Audit
        existing.UpdatedAt = DateTime.UtcNow;
        existing.updatedby = userId;

        // 6. Save
        return await _repository.UpdateAsync(existing);
    }
}