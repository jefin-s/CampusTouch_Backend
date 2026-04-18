//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Students.Commands
//{
//    public class UpdateStudentHandler:IRequestHandler<UpdateStudentCommand,bool>
//    {

//        private readonly IStudentRepository _studentRepository;
//        private readonly ICurrentUserService _currentUserService;
//        public UpdateStudentHandler(IStudentRepository studentRepository, ICurrentUserService currentUserService)
//        {
//            _studentRepository = studentRepository;
//            _currentUserService = currentUserService;
//        }

//        public async Task<bool> Handle(UpdateStudentCommand request,CancellationToken cancellationToken)
//        {
//            var userid = _currentUserService.UserId;

//            var studentexisting = await _studentRepository.GetStudentsById(request.Id);
//            if(studentexisting == null||studentexisting.IsDeleted)
//            {
//                throw new NotFoundException("Student is not Exist");
//            }
//            if (!_currentUserService.IsAdmin)
//            {
//                throw new UnauthorizedAccessException("Only admin can update student");
//            }
//            var AdmissionisExist = await _studentRepository.AdmissionNumberExist(request.AdmissionNumber);
//            if (AdmissionisExist && studentexisting.Id != request.Id)
//                throw new BuisnessRuleException("Admission number is already exist");

//            studentexisting.AdmissionNumber = request.AdmissionNumber;
//            studentexisting.CourseId = request.CourseId;
//            studentexisting.DepartmentId = request.DepartmentId;
//            studentexisting.AdmissionDate = request.AdmissionDate;

//            studentexisting.FirstName = request.FirstName;
//            studentexisting.LastName = request.LastName;
//            studentexisting.DateOfBirth = request.DateOfBirth;
//            studentexisting.Gender = request.Gender;

//            studentexisting.PhoneNumber = request.PhoneNumber;
//            studentexisting.Email = request.Email;
//            studentexisting.Address = request.Address;

//            studentexisting.GuardianName = request.GuardianName;
//            studentexisting.GuardianPhone = request.GuardianPhone;

//            studentexisting.BloodGroup = request.BloodGroup;
//            studentexisting.UpdatedBy = userid;
//             studentexisting.UpdatedAt = DateTime.UtcNow;

//            // 🔥 IMPORTANT LOGIC
//            if (!string.IsNullOrEmpty(request.ProfileImageUrl))
//            {
//                studentexisting.ProfileImageUrl = request.ProfileImageUrl;
//            }

//            await _studentRepository.UpdateStudent(studentexisting);

//            return true;
//        }
//    }
//    }

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Students.Commands
{
    public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, bool>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateStudentHandler> _logger;

        public UpdateStudentHandler(
            IStudentRepository studentRepository,
            ICurrentUserService currentUserService,
            ILogger<UpdateStudentHandler> logger)
        {
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // ✅ Attempt log
            _logger.LogInformation(
                "User {UserId} attempting to update student {StudentId}",
                userId, request.Id);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized update attempt by User {UserId} for Student {StudentId}",
                    userId, request.Id);

                throw new UnauthorizedAccessException("Only admin can update student");
            }

            var student = await _studentRepository.GetStudentsById(request.Id);

            if (student == null || student.IsDeleted)
            {
                _logger.LogWarning(
                    "Update failed: Student {StudentId} not found (User {UserId})",
                    request.Id, userId);

                throw new NotFoundException("Student not found");
            }

            // 🔥 Duplicate check
            var admissionExists = await _studentRepository.AdmissionNumberExist(request.AdmissionNumber);

            if (admissionExists && student.Id != request.Id)
            {
                _logger.LogWarning(
                    "Duplicate admission number update attempt {AdmissionNumber} for Student {StudentId} by User {UserId}",
                    request.AdmissionNumber, request.Id, userId);

                throw new BuisnessRuleException("Admission number already exists");
            }

            // 📝 Capture old values (audit)
            var oldAdmission = student.AdmissionNumber;
            var oldEmail = student.Email;

            // 📝 Update
            student.AdmissionNumber = request.AdmissionNumber;
            student.CourseId = request.CourseId;
            student.DepartmentId = request.DepartmentId;
            student.AdmissionDate = request.AdmissionDate;

            student.FirstName = request.FirstName;
            student.LastName = request.LastName;
            student.DateOfBirth = request.DateOfBirth;
            student.Gender = request.Gender;

            student.PhoneNumber = request.PhoneNumber;
            student.Email = request.Email;
            student.Address = request.Address;

            student.GuardianName = request.GuardianName;
            student.GuardianPhone = request.GuardianPhone;

            student.BloodGroup = request.BloodGroup;
            student.UpdatedBy = userId;
            student.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(request.ProfileImageUrl))
            {
                student.ProfileImageUrl = request.ProfileImageUrl;
            }

            await _studentRepository.UpdateStudent(student);

            // ✅ Audit log (VERY IMPORTANT)
            _logger.LogInformation(
                "User {UserId} updated student {StudentId} successfully. Admission: {OldAdmission} → {NewAdmission}, Email: {OldEmail} → {NewEmail}",
                userId, request.Id, oldAdmission, request.AdmissionNumber, oldEmail, request.Email);

            return true;
        }
    }
}

