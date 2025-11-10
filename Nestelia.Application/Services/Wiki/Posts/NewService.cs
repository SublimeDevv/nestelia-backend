using AutoMapper;
using Nestelia.Application.Interfaces.Auth;
using Nestelia.Application.Interfaces.Storage;
using Nestelia.Application.Interfaces.Wiki.Posts;
using Nestelia.Application.Services.Base;
using Nestelia.Domain.DTO.Wiki.Posts;
using Nestelia.Domain.Entities.Wiki.Posts;
using Nestelia.Domain.Shared;
using Nestelia.Infraestructure.Interfaces.Wiki.Posts;

namespace Nestelia.Application.Services.Wiki.Posts
{
    public class NewService(IMapper mapper, INewRepository newRepository, IStorageService storageService, IAuthService authService) : ServiceBase<New, NewDto>(mapper, newRepository), INewService
    {
        private readonly INewRepository _repository = newRepository;
        private readonly IStorageService _storageService = storageService;
        private readonly IAuthService _authService = authService;

        public async Task<Result<bool>> CreateNewPost(CreateNewDto newDto)
        {

            string ? imagePath = null;
            if (newDto.Image is not null)
            {
                var uploadResult = await _storageService.UploadFileAsync("news", newDto.Image);
                if (!uploadResult.IsSuccess)
                {
                    return Result.Failure<bool>($"Error al subir la imagen: {uploadResult.Error.Message}");
                }

                imagePath = uploadResult.Data!;
            }

            var newPost = new New
            {
                Title = newDto.Title,
                AuthorId = _authService.GetIdentity(),
                Description = newDto.Description,
                Content = newDto.Content,
                CoverImageUrl = imagePath!,
                CreatedAt = DateTime.UtcNow,
            };

            var resultInsert = await _repository.InsertAsync(newPost);

            if (resultInsert == Guid.Empty)
            {
                return Result.Failure<bool>("Error al crear la noticia en la base de datos.");
            }

            return Result.Success(true, "Noticia creada exitosamente.");
        }

        public async Task<Result<bool>> UpdateNewPost(UpdateNewPost newDto)
        {
            var existingNewPost = await _repository.GetSingleAsync(n => n.Id == newDto.Id);
            if (existingNewPost is null)
            {
                return Result.Failure<bool>("Noticia no encontrada.");
            }
            string ? imagePath = existingNewPost.CoverImageUrl;
            if (newDto.Image is not null)
            {
                var uploadResult = await _storageService.UploadFileAsync("news", newDto.Image);
                if (!uploadResult.IsSuccess)
                {
                    return Result.Failure<bool>($"Error al subir la imagen: {uploadResult.Error.Message}");
                }
                imagePath = uploadResult.Data!;
            }
            existingNewPost.Title = newDto.Title;
            existingNewPost.Content = newDto.Content;
            existingNewPost.Description = newDto.Description;
            existingNewPost.CoverImageUrl = imagePath!;
            var resultUpdate = await _repository.UpdateAsync(existingNewPost);
            if (resultUpdate == 0)
            {
                return Result.Failure<bool>("Error al actualizar la noticia en la base de datos.");
            }
            return Result.Success(true, "Noticia actualizada exitosamente.");
        }

        public async Task<Result> GetNewById(string id)
        {
            var newsEntry = await _repository.GetSingleAsync(n => n.Id.ToString() == id);
            if (newsEntry is null)
            {
                return Result.Failure("Noticia no encontrada.");
            }
            if (!string.IsNullOrEmpty(newsEntry.CoverImageUrl))
            {
                var urlResult = await _storageService.GetUrlFile(newsEntry.CoverImageUrl);
                if (urlResult.IsSuccess && urlResult.Data is not null)
                {
                    newsEntry.CoverImageUrl = urlResult.Data;
                }
            }
            return Result.Success(newsEntry, "Noticia obtenida correctamente.");
        }

        public async Task<Result> GetNewsAsync(string param = "", int page = 1, int size = 10)
        {
            page = Math.Max(1, page);
            size = Math.Clamp(size, 1, 100);

            var pagedData = await _repository.GetNewsAsync(param, page, size);

            var imageTasks = pagedData.Items
                .Where(n => !string.IsNullOrEmpty(n.CoverImageUrl))
                .Select(async news =>
                {
                    var urlResult = await _storageService.GetUrlFile(news.CoverImageUrl);
                    if (urlResult.IsSuccess && urlResult.Data is not null)
                    {
                        news.CoverImageUrl = urlResult.Data;
                    }
                });

            await Task.WhenAll(imageTasks);

            return Result.Success(pagedData.Items, "Noticias obtenidas correctamente")
                .With("pagination", new
                {
                    currentPage = page,
                    pageSize = size,
                    totalPages = pagedData.TotalPages,
                    totalCount = pagedData.TotalCount
                });
        }

    }


}