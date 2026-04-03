using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Students.Commands
{
    public record UpdateStudentCommand(
    int Id,
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
) : IRequest<bool>;
}
