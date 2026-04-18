//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Features.Classes.Command;
//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using MediatR;

//public class CreateClassHandler : IRequestHandler<CreateClassCommand, int>
//{
//    private readonly IClassesRepository _repo;
//    private readonly ICurrentUserService _currentUser;
//    public CreateClassHandler(IClassesRepository repo, ICurrentUserService currentUser)
//    {
//        _repo = repo;
//        _currentUser = currentUser;
//    }

//    public async Task<int> Handle(CreateClassCommand request, CancellationToken cancellationToken)
//    {

//        if (!_currentUser.IsAdmin)
//        {
//            throw new BuisnessRuleException("Only admins can create classes");
//        }
//        // 🔥 Check duplicate

//        if (await _repo.ExistsAsync(request.CourseId, request.Year, request.Semester))
//            throw new UnauthorizedException("Class already exists");

//        var model = new Classes
//        {
//            Name = request.Name,
//            DepartmentId = request.DepartmentId,
//            CourseId = request.CourseId,
//            Year = request.Year,
//            Semester = request.Semester
//        };

//        return await _repo.CreateAsync(model);
//    }
//}
using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Features.Classes.Command;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

public class CreateClassHandler : IRequestHandler<CreateClassCommand, int>
{
    private readonly IClassesRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CreateClassHandler> _logger;

    public CreateClassHandler(
        IClassesRepository repo,
        ICurrentUserService currentUser,
        ILogger<CreateClassHandler> logger)
    {
        _repo = repo;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<int> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        _logger.LogInformation("User {UserId} attempting to create class {Name}", userId, request.Name);

        // 🔐 Authorization check
        if (!_currentUser.IsAdmin)
        {
            _logger.LogWarning("Unauthorized class creation attempt by User {UserId}", userId);
            throw new BuisnessRuleException("Only admins can create classes");
        }

        // 🔥 Duplicate check
        if (await _repo.ExistsAsync(request.CourseId, request.Year, request.Semester))
        {
            _logger.LogWarning(
                "Duplicate class creation attempt by User {UserId} for Course {CourseId}, Year {Year}, Semester {Semester}",
                userId, request.CourseId, request.Year, request.Semester);

            throw new UnauthorizedException("Class already exists");
        }

        var model = new Classes
        {
            Name = request.Name,
            DepartmentId = request.DepartmentId,
            CourseId = request.CourseId,
            Year = request.Year,
            Semester = request.Semester
        };

        var classId = await _repo.CreateAsync(model);

        // ✅ Success log (VERY IMPORTANT for audit)
        _logger.LogInformation("User {UserId} created class {ClassId} successfully", userId, classId);

        return classId;
    }
}