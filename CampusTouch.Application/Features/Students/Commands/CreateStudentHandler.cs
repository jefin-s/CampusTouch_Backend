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
    public class CreateStudentHandler:IRequestHandler<CreateStudentCommand,bool>
    {

        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        public CreateStudentHandler(IStudentRepository studentRepository, ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            // 🧱 Create Student entity
            var student = new Student
            {
                

                // 🔥 Important (link to AspNetUsers)
                UserId = userId,

                AdmissionNumber = request.AdmissionNumber,
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
                CreatedAt = DateTime.UtcNow
            };

            // 💾 Save to DB using Dapper
            var result = await _studentRepository.CreateStudentAsync(student);

            // ✅ Return success
            return result > 0;
        }
    }
}
