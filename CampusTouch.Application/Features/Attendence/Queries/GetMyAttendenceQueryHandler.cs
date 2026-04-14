using CampusTouch.Application.Common.Exceptions;
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
    public  class GetMyAttendenceQueryHandler:IRequestHandler<GetMyattendencequery ,IEnumerable<AttendenceViewDto>>
    { 

        private readonly IAttendenceRepository _attendenceRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyAttendenceQueryHandler(IAttendenceRepository attendenceRepository,ICurrentUserService currentUserService)
        {
            _attendenceRepository = attendenceRepository;
            _currentUserService = currentUserService;
        }
        public async Task<IEnumerable<AttendenceViewDto>> Handle(GetMyattendencequery request,CancellationToken cancellationToken)
        {
            var userid=_currentUserService.UserId;
            var studentId= await _attendenceRepository.GetStudentIdByUserIdAsync(userid);
            if (studentId == null)
                throw new NotFoundException("Student not found for the current user.");
            var attendence =await _attendenceRepository.GetAttendenceByStudentId(studentId.Value);
            return attendence;

        }
    }
}
