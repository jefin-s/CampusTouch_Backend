using CampusTouch.Application.Features.Attendence.DTO;
using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Attendence.Queries
{
    public class GetAttendenceByClassQueryHandler : IRequestHandler<GetAttendenceByClassQuery, IEnumerable<AttendenceViewDto>>
    {
        private readonly IAttendenceRepository _repo;

        public GetAttendenceByClassQueryHandler(IAttendenceRepository repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<AttendenceViewDto>> Handle(GetAttendenceByClassQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetAttendanceByClassAsync(request.ClassId, request.Date);
        }
    }
}
