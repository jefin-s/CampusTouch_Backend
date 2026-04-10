using CampusTouch.Application.Features.Authentication.DTOs;
using CampusTouch.Application.Interfaces;
using CampusTouch.Infrastructure.Persistance.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public GoogleAuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
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

            var user = await _userManager.FindByLoginAsync(
                info.LoginProvider,
                info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    // 🆕 New user
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email
                    };

                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                        throw new Exception("User creation failed");
                }

                // 🔗 Link Google login
                await _userManager.AddLoginAsync(user, info);
            }

            // 🔥 CRITICAL FIX: Ensure role for ALL users
            if (!await _userManager.IsInRoleAsync(user, "Applicant"))
            {
                await _userManager.AddToRoleAsync(user, "Applicant");
            }

            // ✅ Get updated roles
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


