using MediatR;


namespace CampusTouch.Application.Features.Staffs.Commands
{
    public  class UpdateStaffCommand:IRequest<bool>
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public  string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Designation { get; set; }

        public int? DepartmentId { get; set; }
    }
}
