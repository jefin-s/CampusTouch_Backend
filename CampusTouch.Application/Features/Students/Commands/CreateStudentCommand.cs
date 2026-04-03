

using MediatR;

namespace CampusTouch.Application.Features.Students.Commands
{
    public  record CreateStudentCommand(
     
        string AdmissionNumber,
        int CourseId,
        int DepartmentId,
        
        DateTime? AdmissionDate,

        string FirstName,
        string? LastName,

        DateTime? DateOfBirth,
        string? Gender,

        string? PhoneNumber,
        string? Email,
        string? Address,

        string? GuardianName,
        string? GuardianPhone,

        string? BloodGroup,
        string? ProfileImageUrl
        ) :IRequest<bool>;
   
}
