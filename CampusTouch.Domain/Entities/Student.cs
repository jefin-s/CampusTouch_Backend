

namespace CampusTouch.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }

        // 🔗 Identity Link (FK → AspNetUsers.Id)
        public string UserId { get; set; }

        // 🎓 Academic Info
        public string AdmissionNumber { get; set; }
        public int  CourseId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime? AdmissionDate { get; set; }

        // 👤 Personal Info
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }

        // 📞 Contact Info
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        // 👨‍👩‍👦 Guardian Info
        public string? GuardianName { get; set; }
        public string? GuardianPhone { get; set; }

        // 🩺 Extra Info
        public string? BloodGroup { get; set; }
        public string? ProfileImageUrl { get; set; }

        // ⚙️ System Fields
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
