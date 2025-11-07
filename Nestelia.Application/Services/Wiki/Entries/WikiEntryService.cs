using AutoMapper;
using Nestelia.Application.Interfaces.Storage;
using Nestelia.Application.Interfaces.Wiki.Entries;
using Nestelia.Application.Services.Base;
using Nestelia.Domain.DTO.Wiki.Entries;
using Nestelia.Domain.Entities.Wiki.Entries;
using Nestelia.Domain.Shared;
using Nestelia.Infraestructure.Interfaces.Wiki.Categories;
using Nestelia.Infraestructure.Interfaces.Wiki.Entries;

namespace Nestelia.Application.Services.Wiki.Entries
{
    public class WikiEntryService(IMapper mapper, IWikiEntryRepository wikiEntryRepository, IStorageService storageService, ICategoryRepository categoryRepository) : ServiceBase<WikiEntry, WikiEntryDto>(mapper, wikiEntryRepository), IWikiEntryService
    {

        private readonly IWikiEntryRepository _wikiEntryRepository = wikiEntryRepository;
        private readonly IStorageService _storageService = storageService;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<Result<bool>> CreateWikiEntry(CreateWikiEntryDto createWikiEntryDto)
        {
            var categoryResult = await _categoryRepository.GetSingleAsync(x => x.Id == createWikiEntryDto.CategoryId);

            if (categoryResult is null)
            {
                return Result.Failure<bool>($"La categoría con Id {createWikiEntryDto.CategoryId} no existe.");
            }

            if (createWikiEntryDto.Image is null || createWikiEntryDto.Image.Length == 0)
            {
                return Result.Failure<bool>("No se proporcionó una imagen para la entrada wiki.");
            }

            var uploadResult = await _storageService.UploadFileAsync(categoryResult.Name, createWikiEntryDto.Image);

            if (!uploadResult.IsSuccess)
            {
                return Result.Failure<bool>($"Error al subir la imagen: {uploadResult.Error.Message}");
            }

            var wikiEntry = new WikiEntry
            {
                Image = uploadResult.Data!,
                Title = createWikiEntryDto.Title,
                Description = createWikiEntryDto.Description,
                CategoryId = createWikiEntryDto.CategoryId,
                CreatedAt = DateTime.UtcNow,
            };

            var resultInsert = await _wikiEntryRepository.InsertAsync(wikiEntry);
            if (resultInsert == Guid.Empty)
            {
                return Result.Failure<bool>("Error al crear la entrada wiki en la base de datos.");
            }

            return Result.Success(true, "Entrada wiki creada exitosamente.");
        }

        public async Task<Result> GetEntriesByCategoryAsync(string category, string param, int page = 1, int size = 10)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return Result.Failure("El parámetro de categoría no puede estar vacío.");
            }

            page = Math.Max(1, page);
            size = Math.Clamp(size, 1, 100);

            var pagedData = await _wikiEntryRepository.GetEntriesByCategory(category, param, page, size);

            var imageTasks = pagedData.Items
                .Where(e => !string.IsNullOrEmpty(e.Image))
                .Select(async entry =>
                {
                    var urlResult = await _storageService.GetUrlFile(entry.Image);
                    if (urlResult.IsSuccess && urlResult.Data is not null)
                    {
                        entry.Image = urlResult.Data;
                    }
                });

            await Task.WhenAll(imageTasks); 

            return Result.Success(pagedData.Items, "Entradas obtenidas correctamente").With("pagination", new { currentPage = page, pageSize = size, totalPages = pagedData.TotalPages, totalCount = 
                pagedData.TotalCount });
        }

    }
}
