using CampusTouch.Application.Common.Exceptions;
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
    public class GetStaffByIdHandler:IRequestHandler<GetStaffByIdQueries,StaffDto?>
    {
        private readonly IStaffRepository _staffRepository;
        public GetStaffByIdHandler(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }
        public async Task<StaffDto?> Handle(GetStaffByIdQueries request, CancellationToken cancellationToken)
        {
            var staff = await _staffRepository.GetStaffById(request.Id);
            if (staff == null)
                 throw new NotFoundException("Staff not found");
            return new StaffDto
            {
                Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                PhoneNumber = staff.PhoneNumber,
                Designation = staff.Designation,
                DepartmentId = staff.DepartmentId,
              
            };
        }

    }
}
