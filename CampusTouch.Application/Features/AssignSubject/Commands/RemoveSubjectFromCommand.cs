using MediatR;

public class RemoveSubjectFromStaffCommand : IRequest<bool>
{
    public int StaffId { get; set; }
    public int SubjectId { get; set; }
}