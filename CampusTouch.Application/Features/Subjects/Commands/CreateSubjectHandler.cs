using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;

public class CreateSubjectHandler : IRequestHandler<CreateSubjectCommand, int>
{
    private readonly ISubjectRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISemsterRepository _semsterRepository;

    public CreateSubjectHandler(ISubjectRepository repository, ISemsterRepository semsterRepository,ICurrentUserService currentUserService)
    {
        _repository = repository;
        _semsterRepository = semsterRepository;
        _currentUserService = currentUserService;
    }


    public async Task<int> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        // 1. Authorization
        if (!_currentUserService.IsAdmin)
            throw new UnauthorizedException("Only Admin can create subject");

        // 2. Validate Semester
        var semester = await _semsterRepository.GetByIdAsync(request.SemesterId);

        if (semester == null || semester.IsDeleted)
            throw new NotFoundException("Semester not found");

        // 3. Duplicate check
        var exists = await _repository.Exist(request.SemesterId, request.Code);

        if (exists)
            throw new BuisnessRuleException("Subject already exists in this semester");

        // 4. Create
        var subject = new Subject
        {
            Name = request.Name,
            Code = request.Code,
            Credits = request.Credits,
            SemesterId = request.SemesterId,
            CreatedAt = DateTime.UtcNow,
            creatdby = userId
        };

        return await _repository.CreateAsync(subject);
    }
}