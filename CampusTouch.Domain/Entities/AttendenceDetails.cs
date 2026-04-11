

namespace CampusTouch.Domain.Entities
{
    public  class AttendenceDetails
    {
        public int Id { get; set; }

        public int AttendanceId { get; set; }
        public int StudentId { get; set; }

        public string Status { get; set; }
        public string Remark { get; set; }

        public DateTime MarkedAt { get; set; }
        public bool IsEdited { get; set; }
    }
}
