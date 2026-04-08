using CampusTouch.Application.Features.Staffs.DTOs;
using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Staffs.Queries
{
    public  class GetAllStaffQueriesHandler:IRequestHandler<GetAllStaffQueries,IEnumerable<StaffDto>>
    {
        private readonly IStaffRepository _staffRepository;
        public GetAllStaffQueriesHandler(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }
        public async Task<IEnumerable<StaffDto>> Handle(GetAllStaffQueries request, CancellationToken cancellationToken)
        {
            var staffs = await _staffRepository.GetAllStaffs();
            return staffs.Select(s => new StaffDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                PhoneNumber = s.PhoneNumber,
                Designation = s.Designation,
                DepartmentId = s.DepartmentId,
                Email = s.Email

            }).ToList();
        }

    }
}
