

namespace CampusTouch.Application.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(Stream filestream, string fileName);
    }
}
