

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace CampusTouch.Application.Features.ApproveStudent.Command
{
    public class ApproveStudentHandler : IRequestHandler<ApproveStudentCommand , bool>
    {
        private readonly IIdentityService _identityService;
        private readonly IStudentRepository _studentRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IDepartementRepository _departementRepository;

        public ApproveStudentHandler(IIdentityService identityService, IStudentRepository studentRepository, IProgramRepository programRepository, IDepartementRepository departementRepository)


        {
            _identityService = identityService;
            _studentRepository = studentRepository;
            _programRepository = programRepository;
            _departementRepository = departementRepository;

        }

        public async Task<bool> Handle(ApproveStudentCommand request, CancellationToken cancellationToken)
        {
            var course = await _programRepository.GetByIdAsync(request.CourseId);
            var department = await _departementRepository.GetByIdAsync(request.DepartmentId);

            if (course == null || department == null)
                throw new NotFoundException("Invalid Course or Department");

            // ✅ Check first
            var existingStudent = await _studentRepository.GetStudentByUserId(request.UserId);

            if (existingStudent != null)
                throw new BuisnessRuleException("Student already exists");

            // Generate admission number
            var sequence = await _studentRepository.GetNextAdmissionSequence(request.DepartmentId);
            var admissionNumber = $"{department.code}{DateTime.UtcNow.Year}{sequence:D3}";

            var AdmissionNumberExists = await _studentRepository.AdmissionNumberExist(admissionNumber);
            if(AdmissionNumberExists)
                throw new BuisnessRuleException("Admission number already exists, try again");

            // Promote role
            await _identityService.PromoteToStudentAsync(request.UserId);

            // Create student
            var student = new Student
            {
                UserId = request.UserId,
                AdmissionNumber = admissionNumber,
                CourseId = request.CourseId,
                DepartmentId = request.DepartmentId,
                FirstName=request.firstName,
                AdmissionDate = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _studentRepository.CreateStudentAsync(student);

            return result > 0;
        }
    }
}
