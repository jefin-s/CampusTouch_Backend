namespace CampusTouch.API.Models.Students
{
    public class CreateStudentRequest
    {
        public string AdmissionNumber { get; set; }
        public int CourseId { get; set; }
        public  int DepartmentId { get; set; }
        public DateTime? AdmissionDate { get; set; }

        public string FirstName { get; set; }
        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public string? GuardianName { get; set; }
        public string? GuardianPhone { get; set; }

        public string? BloodGroup { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }
}
