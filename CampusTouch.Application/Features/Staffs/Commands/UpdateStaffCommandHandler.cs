using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Staffs.Commands
{
    public class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand, bool>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IDepartementRepository _deptRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateStaffCommandHandler(IStaffRepository staffRepository, IDepartementRepository departementRepository, ICurrentUserService currentUserService)
        {
            _staffRepository = staffRepository;
            _deptRepository = departementRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAdmin)
                throw new UnauthorizedException("Only admin can update the staff");

            var staff = await _staffRepository.GetStaffById(request.Id);
            if (staff == null)
                throw new NotFoundException("Staff is Not Found");

            var dept = await _deptRepository.GetByIdAsync(request.DepartmentId);

            if (dept == null)
                throw new NotFoundException("Department is Not Found");

            if(await _staffRepository.IsPhoneNumberExist(request.PhoneNumber,request.Id))
                throw new BuisnessRuleException("Phone number already exist");
            staff.FirstName = request.FirstName;
            staff.LastName = request.LastName;
            staff.PhoneNumber = request.PhoneNumber;
            staff.Designation = request.Designation;
            staff.DepartmentId=request.DepartmentId;
            staff.UpdatedAt = DateTime.UtcNow;
            staff.UpdatedBy = _currentUserService.UserId;

            var count= await _staffRepository.UpdateStaff(staff);
            return count > 0;
        }
    }
}
