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
    public class UpdateStudentHandler:IRequestHandler<UpdateStudentCommand,bool>
    {

        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        public UpdateStudentHandler(IStudentRepository studentRepository, ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateStudentCommand request,CancellationToken cancellationToken)
        {
            var userid = _currentUserService.UserId;
           
            var studentexisting = await _studentRepository.GetStudentsById(request.Id);
            if(studentexisting == null||studentexisting.IsDeleted)
            {
                throw new NotFoundException("Student is not Exist");
            }
            if (!_currentUserService.IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admin can update student");
            }
            var AdmissionisExist = await _studentRepository.AdmissionNumberExist(request.AdmissionNumber);
            if (AdmissionisExist && studentexisting.Id != request.Id)
                throw new BuisnessRuleException("Admission number is already exist");

            studentexisting.AdmissionNumber = request.AdmissionNumber;
            studentexisting.CourseId = request.CourseId;
            studentexisting.DepartmentId = request.DepartmentId;
            studentexisting.AdmissionDate = request.AdmissionDate;

            studentexisting.FirstName = request.FirstName;
            studentexisting.LastName = request.LastName;
            studentexisting.DateOfBirth = request.DateOfBirth;
            studentexisting.Gender = request.Gender;

            studentexisting.PhoneNumber = request.PhoneNumber;
            studentexisting.Email = request.Email;
            studentexisting.Address = request.Address;

            studentexisting.GuardianName = request.GuardianName;
            studentexisting.GuardianPhone = request.GuardianPhone;

            studentexisting.BloodGroup = request.BloodGroup;
            studentexisting.UpdatedBy = userid;
             studentexisting.UpdatedAt = DateTime.UtcNow;

            // 🔥 IMPORTANT LOGIC
            if (!string.IsNullOrEmpty(request.ProfileImageUrl))
            {
                studentexisting.ProfileImageUrl = request.ProfileImageUrl;
            }

            await _studentRepository.UpdateStudent(studentexisting);

            return true;
        }
    }
    }


