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
            private  readonly  IIdentityService _identityService;
            private readonly IEmailService _emailService;


            public CreateStudentHandler(IStudentRepository studentRepository, ICurrentUserService currentUserService, IProgramRepository programRepository, IDepartementRepository departementRepository,IIdentityService identityService, IEmailService emailService)
            {
                _studentRepository = studentRepository;
                _currentUserService = currentUserService;
                _programRepository = programRepository;
                _departementRepository = departementRepository;
                _identityService = identityService;
                _emailService = emailService;
            }
            public async Task<bool> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
            {
                var adminId = _currentUserService.UserId;

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
                var tempPassword = $"Stu@{Guid.NewGuid().ToString().Substring(0, 6)}";
                var userid = await _identityService.RegisterAsync(request.FirstName, request.Email,tempPassword,request.PhoneNumber,AdmissionNumber,"Student");
            
                var student = new Student
                {
                    UserId = userid,// IMPORTANT 🔥 (not current user)

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
                    CreatedBy = adminId
                };

                var result = await _studentRepository.CreateStudentAsync(student);
                if(result>0)
                 await _emailService.SendAsync(request.Email, "Welcome to CampusTouch", $"Your account has been created. Your temporary password is: {tempPassword}");

                return result > 0;
            }
        }
    }
