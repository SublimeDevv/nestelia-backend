using Microsoft.AspNetCore.Mvc;
using Nestelia.Application.Interfaces.Storage;

namespace Nestelia.WebAPI.Controllers.Storage
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageController(IStorageService storageService) : ControllerBase
    {
        [HttpGet("get-storage-info")]
        public async Task<IActionResult> GetStorageInfo()
        {
            var result = await storageService.GetStorageInfo();
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }
            return Ok(result.Data);
        }

        [HttpPost("create-main-bucket")]
        public async Task<IActionResult> CreateMainBucket()
        {
            var result = await storageService.CreateMainBucket();
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }
            return Ok(result.Message);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No se proporcionó ningún archivo." });
            }

            if (string.IsNullOrWhiteSpace(folder))
            {
                return BadRequest(new { message = "Debe especificar una carpeta." });
            }

            byte[] fileContent;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileContent = memoryStream.ToArray();
            }

            var result = await storageService.UploadFileAsync(folder, file.FileName, fileContent);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }

            return Ok(result);
        }

        [HttpGet("get-url-file")]
        public async Task<IActionResult> GetUrlFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest(new { message = "Debe especificar una carpeta y un nombre de archivo." });
            }
            var result = await storageService.GetUrlFile(fileName);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Message);
            }
            return Ok(result);
        }
    }
}