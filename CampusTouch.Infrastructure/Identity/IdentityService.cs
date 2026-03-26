using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Interfaces;
using CampusTouch.Application.Common.Exceptions; // ✅ Important
using CampusTouch.Infrastructure.Persistance.Identity;
using Microsoft.AspNetCore.Identity;

namespace CampusTouch.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJWTService _jwtService;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            IJWTService jWTService)
        {
            _userManager = userManager;
            _jwtService = jWTService;
        }

        // 🔥 REGISTER
        public async Task<string> RegisterAsync(
            string fullName,
            string email,
            string password,
            string phoneNumber)
        {
            var user = new ApplicationUser
            {
                FullName = fullName,
                UserName = email,
                Email = email,
                PhoneNumber = phoneNumber
            };

            var result = await _userManager.CreateAsync(user, password);

            // ❌ OLD: throw new Exception(...)
            // ✅ NEW: Throw custom ValidationException
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException(errors);
            }

            // Assign default role
            var roleResult = await _userManager.AddToRoleAsync(user, "Student");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new BuisnessRuleException(errors);
            }

            return "User Registered Successfully";
        }

        // 🔥 LOGIN
        public async Task<LoginResponseDTO> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new NotFoundException("User not found");

            var isValid = await _userManager.CheckPasswordAsync(user, password);

           
            if (!isValid)
                throw new UnauthorizedException("Invalid password");

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtService.GenerateToken(user.Id, user.Email, roles);

            return new LoginResponseDTO
            {
                Token = token,
                Email = user.Email,
                Roles = roles
            };
        }
    }
}