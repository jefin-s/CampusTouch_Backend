using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Interfaces;
using CampusTouch.Infrastructure.Persistance.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Infrastructure.Identity
{
    public class IdentityService:IIdentityService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJWTService _jwtService;
        public IdentityService(UserManager<ApplicationUser> userManager,IJWTService jWTService)
        {
            _userManager=userManager;
            _jwtService=jWTService;



            
        }
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

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            // 🔥 Assign default role (safe approach)
            await _userManager.AddToRoleAsync(user, "Student");

            return "User Registered Successfully";
        }
        public async Task<LoginResponseDTO> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new Exception("User not found");

            var isValid = await _userManager.CheckPasswordAsync(user, password);

            if (!isValid)
                throw new Exception("Invalid password");

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtService.GenerateToken(user.Id, user.Email, roles);

            // ✅ FIX HERE
            return new LoginResponseDTO
            {
                Token = token,
                Email = user.Email,
                Roles = roles
            };
        }

    }
}
