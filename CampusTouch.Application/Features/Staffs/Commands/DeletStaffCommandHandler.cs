//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Staffs.Commands
//{
//    public class DeletStaffCommandHandler : IRequestHandler<DeleteStafffCommand, bool>
//    {
//        private readonly IStaffRepository _staffRepository;
//        private readonly ICurrentUserService _currentUserService;

//        public DeletStaffCommandHandler(IStaffRepository staffRepository, ICurrentUserService currentUserService)
//        {
//            _staffRepository = staffRepository;
//            _currentUserService = currentUserService;

//        }

//        public async Task<bool> Handle(DeleteStafffCommand request, CancellationToken cancellationToken)
//        {
//            if (!_currentUserService.IsAdmin)
//                throw new UnauthorizedAccessException("Only admin can delete staff.");

//            var staff = await _staffRepository.GetStaffById(request.Id);
//            if (staff == null)
//                throw new NotFoundException("Staff not found.");
//             staff.IsActive=false;
//            staff.DeletedAt = DateTime.UtcNow;
//             staff.DeletedBy = _currentUserService.UserId;

//            var count = await _staffRepository.DeactivateStaff(staff.Id);
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
    public class DeletStaffCommandHandler : IRequestHandler<DeleteStafffCommand, bool>
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeletStaffCommandHandler> _logger;

        public DeletStaffCommandHandler(
            IStaffRepository staffRepository,
            ICurrentUserService currentUserService,
            ILogger<DeletStaffCommandHandler> logger)
        {
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteStafffCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // ✅ Attempt log
            _logger.LogInformation(
                "User {UserId} attempting to delete staff {StaffId}",
                userId, request.Id);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized delete attempt by User {UserId} for Staff {StaffId}",
                    userId, request.Id);

                throw new UnauthorizedAccessException("Only admin can delete staff.");
            }

            var staff = await _staffRepository.GetStaffById(request.Id);

            if (staff == null)
            {
                _logger.LogWarning(
                    "Delete failed: Staff {StaffId} not found (User {UserId})",
                    request.Id, userId);

                throw new NotFoundException("Staff not found.");
            }

            // 🗑 Soft delete / deactivate
            staff.IsActive = false;
            staff.DeletedAt = DateTime.UtcNow;
            staff.DeletedBy = userId;

            var count = await _staffRepository.DeactivateStaff(staff.Id);

            if (count > 0)
            {
                // ✅ Audit log
                _logger.LogInformation(
                    "User {UserId} deleted (deactivated) staff {StaffId} ({Email}) successfully",
                    userId, staff.Id, staff.Email);
            }
            else
            {
                // ❌ Unexpected failure
                _logger.LogError(
                    "Failed to deactivate staff {StaffId} by User {UserId}",
                    staff.Id, userId);
            }

            return count > 0;
        }
    }
}