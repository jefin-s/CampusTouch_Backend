//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Staffs.Commands
//{
//    public  class CreateStaffHandler:IRequestHandler<CreateStaffCommand,bool>
//    {
//        private readonly IStaffRepository _staffrepository;
//        private readonly IIdentityService _identityService;
//        private readonly ICurrentUserService _currentUserService;
//        private readonly IDepartementRepository _departementrepository;
//        private readonly IEmailService _emailService;
//    public CreateStaffHandler(IStaffRepository staffRepository,IIdentityService identityService ,ICurrentUserService currentUserService,IDepartementRepository departementRepository,
//         IEmailService emailservice
//            )
//        {
//            _staffrepository = staffRepository;
//            _identityService = identityService;
//            _currentUserService = currentUserService;
//            _departementrepository = departementRepository;
//        _emailService = emailservice;
//        }

//        public async Task<bool> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
//        {

//            if(!_currentUserService.IsAdmin)
//                throw new UnauthorizedAccessException("Only admin can create staff.");
//            if (string.IsNullOrWhiteSpace(request.FirstName))
//                throw new BuisnessRuleException("First Name is required");
//            if(string.IsNullOrWhiteSpace(request.Email))
//                throw new BuisnessRuleException("Email is required");
//            if (!request.Email.Contains("@"))
//            throw new BuisnessRuleException("Email is not valid");

//              if(await _staffrepository.IsEmailExist(request.Email))
//                throw new BuisnessRuleException("Email already exists");
//            var course= await  _departementrepository.GetByIdAsync(request.DepartmentId);
//            if (course == null)
//                throw new BuisnessRuleException("Department not found");

//            var tempPassword = $"Staff@{Guid.NewGuid().ToString().Substring(0, 6)}";
//            var userId = await  _identityService.RegisterAsync(request.FirstName,request.Email,tempPassword,request.PhoneNumber,request.Email,"Staff");

//            if(string.IsNullOrEmpty(userId))
//                throw new Exception("Failed to create user account for staff.");
//            var staff = new Staff
//            {
//                FirstName = request.FirstName,
//                LastName = request.LastName,
//                Email = request.Email,
//                PhoneNumber = request.PhoneNumber,
//                DepartmentId = request.DepartmentId,
//                Designation = request.Designation,
//                JoiningDate=DateTime.UtcNow,
//                IsActive = true,
//                CreatedAt = DateTime.UtcNow,
//                CreatedBy = _currentUserService.UserId,
//                UserId = userId
//            };

//            var result = await _staffrepository.CreateNewStaff(staff);
//        if(result>0)
//            await _emailService.SendAsync(request.Email, "Welcome to CampusTouch", $"Your account has been created. Your temporary password is: {tempPassword}");
//            return result > 0;
//        }

//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Staffs.Commands
{
    public class CreateStaffHandler : IRequestHandler<CreateStaffCommand, bool>
    {
        private readonly IStaffRepository _staffrepository;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDepartementRepository _departementrepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<CreateStaffHandler> _logger;

        public CreateStaffHandler(
            IStaffRepository staffRepository,
            IIdentityService identityService,
            ICurrentUserService currentUserService,
            IDepartementRepository departementRepository,
            IEmailService emailservice,
            ILogger<CreateStaffHandler> logger)
        {
            _staffrepository = staffRepository;
            _identityService = identityService;
            _currentUserService = currentUserService;
            _departementrepository = departementRepository;
            _emailService = emailservice;
            _logger = logger;
        }

        public async Task<bool> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // ✅ Attempt log
            _logger.LogInformation(
                "User {UserId} attempting to create staff {Email} in Department {DepartmentId}",
                userId, request.Email, request.DepartmentId);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning("Unauthorized staff creation attempt by User {UserId}", userId);
                throw new UnauthorizedAccessException("Only admin can create staff.");
            }

            // ⚠ Validation logs (optional but useful)
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                _logger.LogWarning("Staff creation failed: FirstName missing (User {UserId})", userId);
                throw new BuisnessRuleException("First Name is required");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                _logger.LogWarning("Staff creation failed: Email missing (User {UserId})", userId);
                throw new BuisnessRuleException("Email is required");
            }

            if (!request.Email.Contains("@"))
            {
                _logger.LogWarning("Invalid email format for {Email} (User {UserId})", request.Email, userId);
                throw new BuisnessRuleException("Email is not valid");
            }

            if (await _staffrepository.IsEmailExist(request.Email))
            {
                _logger.LogWarning("Duplicate staff email attempt: {Email} (User {UserId})", request.Email, userId);
                throw new BuisnessRuleException("Email already exists");
            }

            var department = await _departementrepository.GetByIdAsync(request.DepartmentId);

            if (department == null)
            {
                _logger.LogWarning("Department {DepartmentId} not found for staff creation (User {UserId})",
                    request.DepartmentId, userId);

                throw new BuisnessRuleException("Department not found");
            }

            var tempPassword = $"Staff@{Guid.NewGuid().ToString().Substring(0, 6)}";

            // 🔑 Identity creation
            var newUserId = await _identityService.RegisterAsync(
                request.FirstName,
                request.Email,
                tempPassword,
                request.PhoneNumber,
                request.Email,
                "Staff");

            if (string.IsNullOrEmpty(newUserId))
            {
                _logger.LogError("Failed to create identity user for staff {Email}", request.Email);
                throw new Exception("Failed to create user account for staff.");
            }

            var staff = new Staff
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DepartmentId = request.DepartmentId,
                Designation = request.Designation,
                JoiningDate = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UserId = newUserId
            };

            var result = await _staffrepository.CreateNewStaff(staff);

            if (result > 0)
            {
                _logger.LogInformation(
                    "User {UserId} created staff {StaffEmail} successfully with StaffId {StaffId}",
                    userId, request.Email, newUserId);

                // 📧 Email (do not log password)
                await _emailService.SendAsync(
                    request.Email,
                    "Welcome to CampusTouch",
                    $"Your account has been created. Your temporary password is: {tempPassword}");
            }
            else
            {
                _logger.LogError("Failed to save staff record for {Email}", request.Email);
            }

            return result > 0;
        }
    }
}