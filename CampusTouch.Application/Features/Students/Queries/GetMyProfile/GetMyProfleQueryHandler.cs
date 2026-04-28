using MediatR;
using CampusTouch.Application.Features.Students.DTOs;
using CampusTouch.Application.Interfaces;

namespace CampusTouch.Application.Features.Students.Queries.GetMyProfile
{
    public class GetMyProfileQueryHandler
        : IRequestHandler<GetMyProfileQuery, StudentMyProfileDTO>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUser;

        public GetMyProfileQueryHandler(
            IStudentRepository studentRepository,
            ICurrentUserService currentUser)
        {
            _studentRepository = studentRepository;
            _currentUser = currentUser;
        }

        public async Task<StudentMyProfileDTO> Handle(
            GetMyProfileQuery request,
            CancellationToken cancellationToken)
        {   
            // 🔥 Get logged-in user ID
            var userId = _currentUser.UserId;

            if (string.IsNullOrEmpty(userId))
                throw new Exception("User not authenticated");

            // 🔥 Fetch student by UserId
            var student = await _studentRepository.GetStudentByUserId(userId);

            if (student == null)
                throw new Exception("Student profile not found");

            return student;
            // 🔥 Map to DTO
            //return new StudentMyProfileDTO
            //{
            //    Id = student.Id,
            //    FirstName = student.FirstName,
            //    LastName = student.LastName,
            //    Email = student.Email,
            //    PhoneNumber = student.PhoneNumber,
            //    Address = student.Address,
            //    Gender = student.Gender,
            //    DateOfBirth = student.DateOfBirth,
            //    AdmissionDate = student.AdmissionDate,
            //    BloodGroup = student.BloodGroup,
            //    ProfileImageUrl = student.ProfileImageUrl,
            //    CourseId = student.CourseId,
            //    DepartmentId = student.DepartmentId
            //};
        }
    }
}