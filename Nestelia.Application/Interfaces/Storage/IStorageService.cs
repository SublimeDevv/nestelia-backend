using Microsoft.AspNetCore.Http;
using Nestelia.Domain.Shared;

namespace Nestelia.Application.Interfaces.Storage
{
    public interface IStorageService
    {
        Task<Result<string>> UploadFileAsync(string folder, IFormFile file);
        Task<Result<bool>> CreateMainBucket();
        Task<Result<string>> GetStorageInfo();
        Task<Result<string>> GetUrlFile(string fileName);
    }
}
