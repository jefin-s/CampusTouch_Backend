using MediatR;

namespace CampusTouch.Application.Features.Semester.Commands
{
   

    public record UpdateSemesterCommand(int Id, string Name, int OrderNumber, int CourseId) : IRequest<bool>;
}
