using FluentValidation;
using CampusTouch.Application.Features.Students.Commands;

namespace CampusTouch.Application.Features.Students.Validators
{
    public class CreateStudentCommandValidator
        : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentCommandValidator()
        {
          
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .MaximumLength(100)
                .WithMessage("First name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s]+$")
                .WithMessage("First name can contain only letters and spaces");

           
            RuleFor(x => x.LastName)
                .MaximumLength(100)
                .WithMessage("Last name cannot exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

           
            RuleFor(x => x.CourseId)
                .GreaterThan(0)
                .WithMessage("Valid course is required");

           
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0)
                .WithMessage("Valid department is required");

           
            RuleFor(x => x.AdmissionDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .When(x => x.AdmissionDate.HasValue)
                .WithMessage("Admission date cannot be in the future");

            // =========================
            // Date Of Birth
            // =========================
            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.UtcNow.AddYears(-3))
                .When(x => x.DateOfBirth.HasValue)
                .WithMessage("Invalid date of birth");

            // =========================
            // Email
            // =========================
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Invalid email address")
                .MaximumLength(150)
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            // =========================
            // Phone Number
            // =========================
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[0-9]{10,15}$")
                .WithMessage("Phone number must contain 10 to 15 digits")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            // =========================
            // Guardian Phone
            // =========================
            RuleFor(x => x.GuardianPhone)
                .Matches(@"^[0-9]{10,15}$")
                .WithMessage("Guardian phone must contain 10 to 15 digits")
                .When(x => !string.IsNullOrWhiteSpace(x.GuardianPhone));

            // =========================
            // Address
            // =========================
            RuleFor(x => x.Address)
                .MaximumLength(500)
                .WithMessage("Address cannot exceed 500 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            // =========================
            // Gender
            // =========================
            RuleFor(x => x.Gender)
                .Must(g =>
                    g == "Male" ||
                    g == "Female" ||
                    g == "Other")
                .When(x => !string.IsNullOrWhiteSpace(x.Gender))
                .WithMessage("Invalid gender value");

            // =========================
            // Blood Group
            // =========================
            RuleFor(x => x.BloodGroup)
                .Must(bg =>
                    new[]
                    {
                        "A+","A-",
                        "B+","B-",
                        "AB+","AB-",
                        "O+","O-"
                    }.Contains(bg))
                .When(x => !string.IsNullOrWhiteSpace(x.BloodGroup))
                .WithMessage("Invalid blood group");

            // =========================
            // Guardian Name
            // =========================
            RuleFor(x => x.GuardianName)
                .MaximumLength(100)
                .WithMessage("Guardian name cannot exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.GuardianName));
        }
    }
}