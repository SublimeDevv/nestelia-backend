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
    public class PostService(IMapper mapper, IPostRepository postRepository, IStorageService storageService, IAuthService authService) : ServiceBase<Post, PostDto>(mapper, postRepository), IPostService
    {
        private readonly IPostRepository _postRepository = postRepository;
        private readonly IStorageService _storageService = storageService;
        private readonly IAuthService _authService = authService;

        public async Task<Result<bool>> CreatePost(CreatePostDto postDto)
        {
            string? imagePath = null;
            if (postDto.Image is not null)
            {
                var uploadResult = await _storageService.UploadFileAsync("posts", postDto.Image);
                if (!uploadResult.IsSuccess)
                {
                    return Result.Failure<bool>($"Error al subir la imagen: {uploadResult.Error.Message}");
                }
                imagePath = uploadResult.Data!;

            }

            var post = new Post
            {
                Title = postDto.Title,
                AuthorId = _authService.GetIdentity(),
                Description = postDto.Description,
                Content = postDto.Content,
                CoverImageUrl = imagePath!,
                CreatedAt = DateTime.UtcNow,
            };

            var resultInsert = await _postRepository.InsertAsync(post);

            if (resultInsert == Guid.Empty)
            {
                return Result.Failure<bool>("Error al crear la noticia en la base de datos.");
            }

            return Result.Success(true, "Noticia creada exitosamente.");
        }

        public async Task<Result<bool>> UpdatePost(UpdatePostDto postDto)
        {
            var existingPost = await _postRepository.GetSingleAsync(n => n.Id == postDto.Id);
            if (existingPost is null)
            {
                return Result.Failure<bool>("Noticia no encontrada.");
            }
            string? imagePath = existingPost.CoverImageUrl;
            if (postDto.Image is not null)
            {
                var uploadResult = await _storageService.UploadFileAsync("posts", postDto.Image);
                if (!uploadResult.IsSuccess)
                {
                    return Result.Failure<bool>($"Error al subir la imagen: {uploadResult.Error.Message}");
                }
                imagePath = uploadResult.Data!;
            }
            existingPost.Title = postDto.Title;
            existingPost.Content = postDto.Content;
            existingPost.Description = postDto.Description;
            existingPost.CoverImageUrl = imagePath!;
            var resultUpdate = await _postRepository.UpdateAsync(existingPost);
            if (resultUpdate == 0)
            {
                return Result.Failure<bool>("Error al actualizar la noticia en la base de datos.");
            }
            return Result.Success(true, "Noticia actualizada exitosamente.");
        }

        public async Task<Result> GetPostById(string id)
        {
            var postEntry = await _postRepository.GetSingleAsync(n => n.Id.ToString() == id);
            if (postEntry is null)
            {
                return Result.Failure("Noticia no encontrada.");
            }
            if (!string.IsNullOrEmpty(postEntry.CoverImageUrl))
            {
                var urlResult = await _storageService.GetUrlFile(postEntry.CoverImageUrl);
                if (urlResult.IsSuccess && urlResult.Data is not null)
                {
                    postEntry.CoverImageUrl = urlResult.Data;
                }
            }
            return Result.Success(postEntry, "Noticia obtenida correctamente.");
        }

        public async Task<Result> GetPostsAsync(string param = "", int page = 1, int size = 10)
        {
            page = Math.Max(1, page);
            size = Math.Clamp(size, 1, 100);
            var pagedData = await _postRepository.GetPostsAsync(param, page, size);
            var imageTasks = pagedData.Items
                .Where(n => !string.IsNullOrEmpty(n.CoverImageUrl))
                .Select(async post =>
                {
                    var urlResult = await _storageService.GetUrlFile(post.CoverImageUrl);
                    if (urlResult.IsSuccess && urlResult.Data is not null)
                    {
                        post.CoverImageUrl = urlResult.Data;
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
