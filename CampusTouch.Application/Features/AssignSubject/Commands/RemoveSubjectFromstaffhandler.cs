using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;

public class RemoveSubjectFromStaffHandler : IRequestHandler<RemoveSubjectFromStaffCommand, bool>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IStaffSubjectRepository _staffSubjectRepository;

    public RemoveSubjectFromStaffHandler(
        IStaffRepository staffRepository,
        IStaffSubjectRepository staffSubjectRepository)
    {
        _staffRepository = staffRepository;
        _staffSubjectRepository = staffSubjectRepository;
    }

    public async Task<bool> Handle(RemoveSubjectFromStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = await _staffRepository.GetStaffById(request.StaffId);
        if (staff == null)
            throw new NotFoundException("Staff not found");

        var exists = await _staffSubjectRepository.Exists(request.StaffId, request.SubjectId);
        if (!exists)
            throw new NotFoundException("Subject not assigned to this staff");

        var result = await _staffSubjectRepository.RemoveAsync(request.StaffId, request.SubjectId);

        return result > 0;
    }
}