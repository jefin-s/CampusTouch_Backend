//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Staffs.Commands
//{
//    public class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand, bool>
//    {
//        private readonly IStaffRepository _staffRepository;
//        private readonly IDepartementRepository _deptRepository;
//        private readonly ICurrentUserService _currentUserService;

//        public UpdateStaffCommandHandler(IStaffRepository staffRepository, IDepartementRepository departementRepository, ICurrentUserService currentUserService)
//        {
//            _staffRepository = staffRepository;
//            _deptRepository = departementRepository;
//            _currentUserService = currentUserService;
//        }

//        public async Task<bool> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
//        {
//            if (!_currentUserService.IsAdmin)
//                throw new UnauthorizedException("Only admin can update the staff");

//            var staff = await _staffRepository.GetStaffById(request.Id);
//            if (staff == null)
//                throw new NotFoundException("Staff is Not Found");

//            var dept = await _deptRepository.GetByIdAsync(request.DepartmentId);

//            if (dept == null)
//                throw new NotFoundException("Department is Not Found");

//            if(await _staffRepository.IsPhoneNumberExist(request.PhoneNumber,request.Id))
//                throw new BuisnessRuleException("Phone number already exist");
//            staff.FirstName = request.FirstName;
//            staff.LastName = request.LastName;
//            staff.PhoneNumber = request.PhoneNumber;
//            staff.Designation = request.Designation;
//            staff.DepartmentId=request.DepartmentId;
//            staff.UpdatedAt = DateTime.UtcNow;
//            staff.UpdatedBy = _currentUserService.UserId;

//            var count= await _staffRepository.UpdateStaff(staff);
//            return count > 0;
//        }
//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Staffs.Commands
{
    public class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand, bool>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IDepartementRepository _deptRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateStaffCommandHandler> _logger;

        public UpdateStaffCommandHandler(
            IStaffRepository staffRepository,
            IDepartementRepository departementRepository,
            ICurrentUserService currentUserService,
            ILogger<UpdateStaffCommandHandler> logger)
        {
            _staffRepository = staffRepository;
            _deptRepository = departementRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // ✅ Attempt log
            _logger.LogInformation(
                "User {UserId} attempting to update staff {StaffId}",
                userId, request.Id);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized update attempt by User {UserId} for Staff {StaffId}",
                    userId, request.Id);

                throw new UnauthorizedException("Only admin can update staff");
            }

            var staff = await _staffRepository.GetStaffById(request.Id);

            if (staff == null)
            {
                _logger.LogWarning(
                    "Update failed: Staff {StaffId} not found (User {UserId})",
                    request.Id, userId);

                throw new NotFoundException("Staff not found");
            }

            var dept = await _deptRepository.GetByIdAsync(request.DepartmentId);

            if (dept == null)
            {
                _logger.LogWarning(
                    "Update failed: Department {DepartmentId} not found for Staff {StaffId} (User {UserId})",
                    request.DepartmentId, request.Id, userId);

                throw new NotFoundException("Department not found");
            }

            if (await _staffRepository.IsPhoneNumberExist(request.PhoneNumber, request.Id))
            {
                _logger.LogWarning(
                    "Duplicate phone number update attempt for Staff {StaffId} by User {UserId}",
                    request.Id, userId);

                throw new BuisnessRuleException("Phone number already exists");
            }

            // 📝 Capture old values (for audit)
            var oldPhone = staff.PhoneNumber;
            var oldDepartment = staff.DepartmentId;

            // 📝 Update
            staff.FirstName = request.FirstName;
            staff.LastName = request.LastName;
            staff.PhoneNumber = request.PhoneNumber;
            staff.Designation = request.Designation;
            staff.DepartmentId = request.DepartmentId;
            staff.UpdatedAt = DateTime.UtcNow;
            staff.UpdatedBy = userId;

            var count = await _staffRepository.UpdateStaff(staff);

            if (count > 0)
            {
                // ✅ Audit log
                _logger.LogInformation(
                    "User {UserId} updated staff {StaffId} successfully. Phone: {OldPhone} → {NewPhone}, Dept: {OldDept} → {NewDept}",
                    userId, request.Id, oldPhone, request.PhoneNumber, oldDepartment, request.DepartmentId);
            }
            else
            {
                // ❌ Unexpected failure
                _logger.LogError(
                    "Failed to update staff {StaffId} by User {UserId}",
                    request.Id, userId);
            }

            return count > 0;
        }
    }
}