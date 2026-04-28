
namespace CampusTouch.Application.Features.Students.DTOs
{
    public class StudentMyProfileDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public int CourseId { get; set; }
        public string CourseName { get; set; }   // ✅ NEW

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } // ✅ NEW

        public DateTime? AdmissionDate { get; set; }

        public string Address { get; set; }
        public string BloodGroup { get; set; }

        public string ProfileImageUrl { get; set; }
    }
}

