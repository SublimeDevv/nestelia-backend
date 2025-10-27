using Microsoft.Extensions.Configuration;
using Nestelia.Application.Interfaces.Storage;
using Nestelia.Domain.Shared;
using Supabase;

namespace Nestelia.Application.Services.Storage
{
    public class StorageService(Client supabaseClient, IConfiguration configuration) : IStorageService
    {
        private readonly Client supabase = supabaseClient;
        private readonly string bucketName = configuration.GetValue<string>("Supabase:MainBucket")!;

        public async Task<Result<bool>> CreateMainBucket()
        {
            var bucket = await supabase.Storage.GetBucket(bucketName);
            if (bucket is not null)
            {
                return Result.Success(true, "Bucket already exists.");
            }

            var newBucket = await supabase.Storage.CreateBucket(bucketName);
            if (newBucket is null)
            {
                return Result.Failure<bool>("Failed to create bucket");
            }

            return Result.Success(true, "Bucket created successfully.");
        }

        public async Task<Result<string>> GetStorageInfo()
        {
            var buckets = await supabase.Storage.ListBuckets();
            if (buckets is null)
            {
                return Result.Failure<string>("Failed to retrieve storage info.");
            }
            var info = $"Total Buckets: {buckets.Count}";
            return Result.Success(info);
        }

        public async Task<Result<bool>> UploadFileAsync(string folder, string fileName, byte[] content)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(extension))
            {
                return Result.Failure<bool>("Solo se permiten archivos JPG, JPEG y PNG.");
            }

            const int maxSizeBytes = 10 * 1024 * 1024;
            if (content.Length > maxSizeBytes)
            {
                return Result.Failure<bool>("El archivo no puede ser mayor a 10 MB.");
            }

            var bucket = await supabase.Storage.GetBucket(bucketName);
            if (bucket is null)
            {
                return Result.Failure<bool>("Bucket does not exist.");
            }

            var storagePath = $"{folder}/{fileName}";

            var uploadResult = await supabase.Storage.From(bucketName).Upload(content, storagePath);
            if (uploadResult is null)
            {
                return Result.Failure<bool>("File upload failed.");
            }

            return Result.Success(true, "File uploaded successfully.");
        }

        public async Task<Result<string>> GetUrlFile(string fileName)
        {
            var bucket = await supabase.Storage.GetBucket(bucketName);
            if (bucket is null)
            {
                return Result.Failure<string>("Bucket does not exist.");
            }
            var url = supabase.Storage.From(bucketName).GetPublicUrl(fileName);
            if (string.IsNullOrEmpty(url))
            {
                return Result.Failure<string>("Failed to get file URL.");
            }
            return Result.Success(url);
        }

    }

}
