using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Students.Commands
{
    public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, bool>
    {

        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProgramRepository _programRepository;
        private readonly IDepartementRepository _departementRepository;
       
        public CreateStudentHandler(IStudentRepository studentRepository, ICurrentUserService currentUserService, IProgramRepository programRepository, IDepartementRepository departementRepository)
        {
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
            _programRepository = programRepository;
            _departementRepository = departementRepository;
        }
        public async Task<bool> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // 1. Authorization
            if (!_currentUserService.IsAdmin)
                throw new UnauthorizedException("Only Admin can create student");

          


            var course = await _programRepository.GetByIdAsync(request.CourseId);
            var department = await _departementRepository.GetByIdAsync(request.DepartmentId);

            if (course == null || department == null||course.IsDeleted||department.isDeleted)
                throw new NotFoundException("Invalid Course or Department");

            var sequence= await _studentRepository.GetNextAdmissionSequence(request.DepartmentId);
            var AdmissionNumber = $"{department.code}{DateTime.UtcNow.Year}{sequence:D3}";
            // 2. Duplicate check
            var exists = await _studentRepository.AdmissionNumberExist(AdmissionNumber);
            if (exists)
                throw new BuisnessRuleException("Student with this admission number already exists");

            if(string.IsNullOrWhiteSpace(request.Email))
            {
                throw new BuisnessRuleException("Email is required");
            }
            // 4. Create entity

            var student = new Student
            {
                UserId = null,// IMPORTANT 🔥 (not current user)

                AdmissionNumber = AdmissionNumber,
                CourseId = request.CourseId,
                DepartmentId = request.DepartmentId,

                AdmissionDate = request.AdmissionDate,

                FirstName = request.FirstName,
                LastName = request.LastName,

                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,

                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,

                GuardianName = request.GuardianName,
                GuardianPhone = request.GuardianPhone,

                BloodGroup = request.BloodGroup,
                ProfileImageUrl = request.ProfileImageUrl,

                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var result = await _studentRepository.CreateStudentAsync(student);

            return result > 0;
        }
    }
}
