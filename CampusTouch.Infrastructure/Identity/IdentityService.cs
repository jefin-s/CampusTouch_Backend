    using CampusTouch.Application.Common.Exceptions; 
    using CampusTouch.Application.Features.Authentication.DTOs;
    using CampusTouch.Application.Interfaces;
    using CampusTouch.Domain.Entities;
    using CampusTouch.Infrastructure.Persistance.Identity;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Cryptography;
using  CampusTouch.Application.Common.Constants;

namespace CampusTouch.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJWTService _jwtService;
        private readonly ApplicationDbContext _context;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            IJWTService jWTService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _jwtService = jWTService;
            _context = context;


        }


        public async Task<string> RegisterAsync(
            string fullName,
            string email,
            string password,
            string phoneNumber,
            string username,
            string role)
        {
            var user = new ApplicationUser
            {
                FullName = fullName,
                UserName = username,
                Email = email,
                PhoneNumber = phoneNumber
            };

            if (await _userManager.FindByEmailAsync(email) != null)
            {
                throw new ValidationException("Email is Already Exist");
            }
            if (await _userManager.FindByNameAsync(username) != null)
            {
                throw new ValidationException("UserName is  Already Exist");
            }
            if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber))
            {
                throw new ValidationException("This Phone number is Already Taken");
            }

            var result = await _userManager.CreateAsync(user, password);


            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException(errors);
            }


            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new BuisnessRuleException(errors);
            }

            return user.Id;
        }


        public async Task<LoginResponseDTO> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new NotFoundException("User not found");

            var isValid = await _userManager.CheckPasswordAsync(user, password);

            if (!isValid)
                throw new UnauthorizedException("Invalid password or Email");

            var roles = await _userManager.GetRolesAsync(user);


            var accessToken = _jwtService.GenerateToken(user.Id, user.Email, roles);


            var refreshToken = GenerateRefreshToken();


            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                Revoked = false
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();


            return new LoginResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Email = user.Email,
                Roles = roles
            };
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }
        public async Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var existingToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (existingToken == null)
                throw new UnauthorizedException("Invalid refresh token");

            if (existingToken.Revoked)
                throw new UnauthorizedException("Token revoked");

            if (existingToken.Expires < DateTime.UtcNow)
                throw new UnauthorizedException("Token expired");

            var user = await _userManager.FindByIdAsync(existingToken.UserId);

            if (user == null)
                throw new NotFoundException("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            // 🔥 rotate old token
            existingToken.Revoked = true;

            // 🔥 generate new refresh token
            var newRefreshToken = GenerateRefreshToken();

            var newToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = newRefreshToken,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                Revoked = false
            };

            _context.RefreshTokens.Add(newToken);

            // 🔥 generate new access token
            var newAccessToken = _jwtService.GenerateToken(user.Id, user.Email, roles);

            await _context.SaveChangesAsync();

            return new LoginResponseDTO
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Email = user.Email,
                Roles = roles
            };
        }

        //public async Task PromoteToStudentAsync(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user == null)
        //        throw new NotFoundException("User not found");

        //    var currentRoles = await _userManager.GetRolesAsync(user);

        //    // Already a Student
        //    if (currentRoles.Contains(Roles.Student))
        //        throw new ValidationException("User is already a Student");

        //    // Prevent Admin misuse
        //    if (currentRoles.Contains(Roles.Admin))
        //        throw new ValidationException("Admin cannot be promoted to Student");

        //    // Must be Applicant
        //    if (!currentRoles.Contains(Roles.Applicant))
        //        throw new ValidationException("User is not an Applicant");

        //    // 🔥 Remove Applicant role
        //    var removeResult = await _userManager.RemoveFromRoleAsync(user, Roles.Applicant);

        //    if (!removeResult.Succeeded)
        //    {
        //        var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
        //        throw new BuisnessRuleException(errors);
        //    }

        //    // 🔥 Add Student role
        //    var addResult = await _userManager.AddToRoleAsync(user, Roles.Student);

        //    if (!addResult.Succeeded)
        //    {
        //        var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
        //        throw new BuisnessRuleException(errors);
        //    }
        //}
        public async Task PromoteToStudentAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            // ✅ Idempotent (VERY IMPORTANT)
            if (roles.Contains(Roles.Student))
                return;

            // ❌ Prevent Admin misuse
            if (roles.Contains(Roles.Admin))
                throw new ValidationException("Admin cannot be promoted to Student");

            // ❌ Must be Applicant
            if (!roles.Contains(Roles.Applicant))
                throw new ValidationException("User is not an Applicant");

            // 🔥 Remove Applicant
            var removeResult = await _userManager.RemoveFromRoleAsync(user, Roles.Applicant);

            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                throw new BuisnessRuleException(errors);
            }

            // 🔥 Add Student
            var addResult = await _userManager.AddToRoleAsync(user, Roles.Student);

            if (!addResult.Succeeded)
            {
                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                throw new BuisnessRuleException(errors);
            }
        }
    }
}