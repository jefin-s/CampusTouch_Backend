using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.ApproveStudent.Command
{
    public class ApproveStudentHandler : IRequestHandler<ApproveStudentCommand, bool>
    {
        private readonly IIdentityService _identityService;
        private readonly IStudentRepository _studentRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IDepartementRepository _departementRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ApproveStudentHandler> _logger;

        public ApproveStudentHandler(
            IIdentityService identityService,
            IStudentRepository studentRepository,
            IProgramRepository programRepository,
            IDepartementRepository departementRepository,
            ICurrentUserService currentUserService,
            ILogger<ApproveStudentHandler> logger)
        {
            _identityService = identityService;
            _studentRepository = studentRepository;
            _programRepository = programRepository;
            _departementRepository = departementRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(ApproveStudentCommand request, CancellationToken cancellationToken)
        {
            var adminId = _currentUserService.UserId;

            // ✅ Attempt log
            _logger.LogInformation(
                "User {UserId} attempting to approve student for UserId {TargetUserId}",
                adminId, request.UserId);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized approve student attempt by User {UserId}",
                    adminId);

                throw new UnauthorizedException("Only admin can approve student");
            }

            // 🔍 Validate course + department
            var course = await _programRepository.GetByIdAsync(request.CourseId);
            var department = await _departementRepository.GetByIdAsync(request.DepartmentId);

            if (course == null || department == null)
            {
                _logger.LogWarning(
                    "Approval failed: Invalid Course/Department (User {UserId}, Course {CourseId}, Dept {DeptId})",
                    adminId, request.CourseId, request.DepartmentId);

                throw new NotFoundException("Invalid course or department");
            }

            // 🔁 Check existing student
            var existingStudent = await _studentRepository.GetStudentByUserId(request.UserId);

            if (existingStudent != null)
            {
                _logger.LogWarning(
                    "Approval failed: Student already exists for UserId {TargetUserId}",
                    request.UserId);

                throw new BuisnessRuleException("Student already exists");
            }

            // 🔢 Admission number generation
            var sequence = await _studentRepository.GetNextAdmissionSequence(request.DepartmentId);
            var admissionNumber = $"{department.code}{DateTime.UtcNow.Year}{sequence:D3}";

            var admissionExists = await _studentRepository.AdmissionNumberExist(admissionNumber);

            if (admissionExists)
            {
                _logger.LogWarning(
                    "Duplicate admission number generated {AdmissionNumber} by User {UserId}",
                    admissionNumber, adminId);

                throw new BuisnessRuleException("Admission number already exists");
            }

            try
            {
                // 🔄 Promote role
                await _identityService.PromoteToStudentAsync(request.UserId);

                // 🧱 Create student
                var student = new Student
                {
                    UserId = request.UserId,
                    AdmissionNumber = admissionNumber,
                    CourseId = request.CourseId,
                    DepartmentId = request.DepartmentId,
                    FirstName = request.firstName,
                    AdmissionDate = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = adminId
                };

                var result = await _studentRepository.CreateStudentAsync(student);

                if (result > 0)
                {
                    // ✅ Audit log (VERY IMPORTANT)
                    _logger.LogInformation(
                        "User {UserId} approved student {AdmissionNumber} for UserId {TargetUserId}",
                        adminId, admissionNumber, request.UserId);
                }
                else
                {
                    _logger.LogError(
                        "Failed to create student record for UserId {TargetUserId}",
                        request.UserId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                // ❌ Error log (CRITICAL)
                _logger.LogError(
                    ex,
                    "Error approving student for UserId {TargetUserId} by User {UserId}",
                    request.UserId, adminId);

                throw;
            }
        }
    }
}