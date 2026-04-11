

namespace CampusTouch.Domain.Entities
{
    public class Attendence
    {

        public int Id { get; set; }
        public DateTime AttendanceDate { get; set; }

        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public string StaffId { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
    