using MediatR;


namespace CampusTouch.Application.Features.ApproveStudent.Command
{
    public  class ApproveStudentCommand:IRequest<bool>
    {
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public int DepartmentId { get; set; }

        public string firstName { get; set; }
        public string phoneNumber { get; set; }
         public string email { get; set; }
        
    }
}
