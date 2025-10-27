using Nestelia.Domain.Shared;

namespace Nestelia.Application.Interfaces.Storage
{
    public interface IStorageService
    {
        Task<Result<bool>> UploadFileAsync(string folder, string fileName, byte[] content);
        Task<Result<bool>> CreateMainBucket();
        Task<Result<string>> GetStorageInfo();
        Task<Result<string>> GetUrlFile(string fileName);
    }
}
