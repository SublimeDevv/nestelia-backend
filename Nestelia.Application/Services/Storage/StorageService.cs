using Microsoft.AspNetCore.Http;
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

        public async Task<Result<string>> UploadFileAsync(string folder, IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return Result.Failure<string>("No se proporcionó una imagen para subir");
            }

            if (string.IsNullOrWhiteSpace(folder))
            {
                return Result.Failure<string>("No se proporcionó un nombre para la carpeta");
            }

            byte[] fileContent;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileContent = memoryStream.ToArray();
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(extension))
            {
                return Result.Failure<string>("Solo se permiten archivos JPG, JPEG y PNG.");
            }

            const int maxSizeBytes = 10 * 1024 * 1024;
            if (fileContent.Length > maxSizeBytes)
            {
                return Result.Failure<string>("El archivo no puede ser mayor a 10 MB.");
            }

            var bucket = await supabase.Storage.GetBucket(bucketName);
            if (bucket is null)
            {
                return Result.Failure<string>("Bucket does not exist.");
            }

            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var uniqueId = Guid.NewGuid().ToString("N")[..8]; 
            var uniqueFileName = $"{fileName}_{uniqueId}{extension}";
            var storagePath = $"{folder.ToLower()}/{uniqueFileName}";

            var uploadResult = await supabase.Storage.From(bucketName).Upload(fileContent, storagePath);
            if (uploadResult is null)
            {
                return Result.Failure<string>("File upload failed.");
            }

            return Result.Success(storagePath, "File uploaded successfully.");
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
