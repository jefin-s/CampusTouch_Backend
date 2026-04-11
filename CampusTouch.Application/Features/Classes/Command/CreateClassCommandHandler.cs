using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Features.Classes.Command;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;

public class CreateClassHandler : IRequestHandler<CreateClassCommand, int>
{
    private readonly IClassesRepository _repo;
    private readonly ICurrentUserService _currentUser;
    public CreateClassHandler(IClassesRepository repo, ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {

        if (!_currentUser.IsAdmin)
        {
            throw new BuisnessRuleException("Only admins can create classes");
        }
        // 🔥 Check duplicate

        if (await _repo.ExistsAsync(request.CourseId, request.Year, request.Semester))
            throw new UnauthorizedException("Class already exists");

        var model = new Classes
        {
            Name = request.Name,
            DepartmentId = request.DepartmentId,
            CourseId = request.CourseId,
            Year = request.Year,
            Semester = request.Semester
        };

        return await _repo.CreateAsync(model);
    }
}