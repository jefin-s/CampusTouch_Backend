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
            string username
        );

        Task<LoginResponseDTO> LoginAsync(
          string email,
          string password
      );

        Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken);
    }
}
