using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Authentication.Vaidators
{
    public class RegisterUserCommandValidator:AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.NewRegistertion.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MinimumLength(3).WithMessage("Full name must be at least 3 characters");

            // ✅ Email validation (same as DataAnnotation + regex)
            RuleFor(x => x.NewRegistertion.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .Matches(@"^(?![_.-])[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$")
                .WithMessage("Email cannot start with special characters");

            // ✅ Password validation (strong)
            RuleFor(x => x.NewRegistertion.Password)
                .NotEmpty().WithMessage("Password is required")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#]).{8,}$")
                .WithMessage("Password must be at least 8 chars and contain uppercase, lowercase, number, and special character");

            // ✅ Phone number validation
            RuleFor(x => x.NewRegistertion.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^[6-9]\d{9}$")
                .WithMessage("Enter a valid 10-digit mobile number");

        }
    }
}
