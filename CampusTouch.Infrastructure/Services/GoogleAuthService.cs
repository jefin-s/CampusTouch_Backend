using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Interfaces;
using CampusTouch.Infrastructure.Persistance.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CampusTouch.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public GoogleAuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<GoogleUserDto> AuthenticateAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
                throw new Exception("External login failed");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (email == null)
                throw new Exception("Email not received");

            // 🔍 STEP 1: Check if user already exists with this Google login
            var user = await _userManager.FindByLoginAsync(
                info.LoginProvider,
                info.ProviderKey);

            if (user == null)
            {
                // 🔍 STEP 2: Check if user exists by email
                user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    var fullname = info.Principal.FindFirstValue(ClaimTypes.Name);
                    // 🆕 Create new user
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName=fullname??"googleuser"
                    };

                    var createResult = await _userManager.CreateAsync(user);

                    if (!createResult.Succeeded)
                        throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }

                // 🔥 STEP 3: LINK GOOGLE LOGIN (SAFE CHECK)
                var existingLogin = await _userManager.FindByLoginAsync(
                    info.LoginProvider,
                    info.ProviderKey);

                if (existingLogin == null)
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);

                    if (!addLoginResult.Succeeded)
                        throw new Exception(string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
                }
            }

            // ✅ STEP 4: Ensure role exists
            if (!await _userManager.IsInRoleAsync(user, "Applicant"))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Applicant");

                if (!roleResult.Succeeded)
                    throw new Exception(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            // ✅ STEP 5: Sign in user (for external auth flow)
            await _signInManager.SignInAsync(user, isPersistent: false);

            // ✅ STEP 6: Get roles
            var roles = await _userManager.GetRolesAsync(user);

            return new GoogleUserDto
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles
            };
        }
    }
}