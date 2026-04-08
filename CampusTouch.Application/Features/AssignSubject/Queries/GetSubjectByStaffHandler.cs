using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Features.AssignSubject.Queries;
using CampusTouch.Application.Interfaces;
using MediatR;

public class GetSubjectsByStaffHandler : IRequestHandler<GetSubjectByStaffQueries, List<int>>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IStaffSubjectRepository _staffSubjectRepository;

    public GetSubjectsByStaffHandler(
        IStaffRepository staffRepository,
        IStaffSubjectRepository staffSubjectRepository)
    {
        _staffRepository = staffRepository;
        _staffSubjectRepository = staffSubjectRepository;
    }

    public async Task<List<int>> Handle(GetSubjectByStaffQueries request, CancellationToken cancellationToken)
    {
        var staff = await _staffRepository.GetStaffById(request.StaffId);
        if (staff == null)
            throw new NotFoundException("Staff not found");

        var subjectIds = await _staffSubjectRepository.GetSubjectsByStaffId(request.StaffId);

        return subjectIds;
    }
}