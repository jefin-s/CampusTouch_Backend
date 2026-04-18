//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using MediatR;

//public class CreateSubjectHandler : IRequestHandler<CreateSubjectCommand, int>
//{
//    private readonly ISubjectRepository _repository;
//    private readonly ICurrentUserService _currentUserService;
//    private readonly ISemsterRepository _semsterRepository;

//    public CreateSubjectHandler(ISubjectRepository repository, ISemsterRepository semsterRepository,ICurrentUserService currentUserService)
//    {
//        _repository = repository;
//        _semsterRepository = semsterRepository;
//        _currentUserService = currentUserService;
//    }


//    public async Task<int> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
//    {

//        // 1. Authorization
//        if (!_currentUserService.IsAdmin)
//            throw new UnauthorizedException("Only Admin can create subject");
//        var userId = _currentUserService.UserId;
//        var name= request.Name.Trim();
//        var code = request.Code.Trim().ToUpper();

//        // 2. Validate Semester
//        var semester = await _semsterRepository.GetByIdAsync(request.SemesterId);

//        if (semester == null || semester.IsDeleted)
//            throw new NotFoundException("Semester not found");

//        // 3. Duplicate check
//        var exists = await _repository.Exist(request.SemesterId,code);

//        if (exists)
//            throw new BuisnessRuleException("Subject already exists in this semester");

//        // 4. Create
//        var subject = new Subject
//        {
//            Name = name,
//            Code = code,
//            Credits = request.Credits,
//            SemesterId = request.SemesterId,
//            CreatedAt = DateTime.UtcNow,
//            creatdby = userId
//        };

//        return await _repository.CreateAsync(subject);
//    }
//}


using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

public class CreateSubjectHandler : IRequestHandler<CreateSubjectCommand, int>
{
    private readonly ISubjectRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISemsterRepository _semsterRepository;
    private readonly ILogger<CreateSubjectHandler> _logger;

    public CreateSubjectHandler(
        ISubjectRepository repository,
        ISemsterRepository semsterRepository,
        ICurrentUserService currentUserService,
        ILogger<CreateSubjectHandler> logger)
    {
        _repository = repository;
        _semsterRepository = semsterRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<int> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var name = request.Name.Trim();
        var code = request.Code.Trim().ToUpper();

        // ✅ Attempt log
        _logger.LogInformation(
            "User {UserId} attempting to create subject {Code} in Semester {SemesterId}",
            userId, code, request.SemesterId);

        // 🔐 Authorization
        if (!_currentUserService.IsAdmin)
        {
            _logger.LogWarning(
                "Unauthorized subject creation attempt by User {UserId}",
                userId);

            throw new UnauthorizedException("Only admin can create subject");
        }

        // 🔍 Validate semester
        var semester = await _semsterRepository.GetByIdAsync(request.SemesterId);

        if (semester == null || semester.IsDeleted)
        {
            _logger.LogWarning(
                "Subject creation failed: Semester {SemesterId} not found (User {UserId})",
                request.SemesterId, userId);

            throw new NotFoundException("Semester not found");
        }

        // 🔁 Duplicate check
        var exists = await _repository.Exist(request.SemesterId, code);

        if (exists)
        {
            _logger.LogWarning(
                "Duplicate subject creation attempt {Code} in Semester {SemesterId} by User {UserId}",
                code, request.SemesterId, userId);

            throw new BuisnessRuleException("Subject already exists");
        }

        var subject = new Subject
        {
            Name = name,
            Code = code,
            Credits = request.Credits,
            SemesterId = request.SemesterId,
            CreatedAt = DateTime.UtcNow,
            creatdby = userId
        };

        var subjectId = await _repository.CreateAsync(subject);

        // ✅ Audit log (VERY IMPORTANT)
        _logger.LogInformation(
            "User {UserId} created subject {SubjectId} ({Code}) in Semester {SemesterId}",
            userId, subjectId, code, request.SemesterId);

        return subjectId;
    }
}