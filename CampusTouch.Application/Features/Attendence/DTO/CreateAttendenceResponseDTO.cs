

namespace CampusTouch.Application.Features.Attendence.DTO
{
    public class CreateAttendenceResponseDTO
    {
        public bool Success { get; set; }

        public int AttendanceId { get; set; }

        public string Message { get; set; }

        public int StudentCount { get; set; }
        public List<StudentAttendenceDto> Students { get; set; }
    }
}
