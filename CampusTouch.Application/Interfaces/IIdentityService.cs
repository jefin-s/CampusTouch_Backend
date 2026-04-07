using CampusTouch.Application.Features.Authentication.DTOs;


namespace CampusTouch.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<string> RegisterAsync(
            string fullName,
            string email,
            string password,
            string phoneNumber,
            string username,
            string role
        );

        Task<LoginResponseDTO> LoginAsync(
          string email,
          string password
      );

        Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken);

        Task PromoteToStudentAsync(string userId);
         //Task PromoteToFacultyAsync(string userId, string departmentId);
    }
}
