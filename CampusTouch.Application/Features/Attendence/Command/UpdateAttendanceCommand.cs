using MediatR;

public class UpdateAttendanceCommand : IRequest<bool>
{
    public int AttendanceId { get; set; }

    public List<UpdateAttendanceStudentDTO> Students { get; set; }
}