using CampusTouch.Application.Features.Attendence.DTO;
using MediatR;

public class CreateAttendanceCommand : IRequest<int>
{
    public DateTime Date { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }

    public List<StudentAttendenceDto> Students { get; set; }
}