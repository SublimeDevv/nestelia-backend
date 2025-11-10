using AutoMapper;
using Nestelia.Application.Interfaces.Storage;
using Nestelia.Application.Interfaces.Wiki.Categories;
using Nestelia.Application.Services.Base;
using Nestelia.Domain.Common.ViewModels.Category;
using Nestelia.Domain.DTO.Wiki.Categories;
using Nestelia.Domain.Entities.Wiki.Categories;
using Nestelia.Domain.Shared;
using Nestelia.Infraestructure.Interfaces.Wiki.Categories;

namespace Nestelia.Application.Services.Wiki.Categories
{
    public class CategoryService(ICategoryRepository repository, IStorageService storageService, IMapper mapper) : ServiceBase<Category, CategoryDto>(mapper, repository), ICategoryService
    {
        private readonly ICategoryRepository _repository = repository;
        private readonly IStorageService _storageService = storageService;

        public async Task<Result<bool>> CreateCategory(CreateCategoryDto categoryDto)
        {
            string? iconPath = null;

            if (categoryDto.Icon is not null)
            {
                var uploadResult = await _storageService.UploadFileAsync(categoryDto.Name, categoryDto.Icon);

                if (!uploadResult.IsSuccess)
                {
                    return Result.Failure<bool>($"Error al subir la imagen: {uploadResult.Error.Message}");
                }

                iconPath = uploadResult.Data!;
            }

            var category = new Category
            {
                Name = categoryDto.Name,
                DisplayName = categoryDto.DisplayName,
                Description = categoryDto.Description,
                Icon = iconPath!,
                CreatedAt = DateTime.UtcNow,
            };

            var resultInsert = await _repository.InsertAsync(category);

            if (resultInsert == Guid.Empty)
            {
                return Result.Failure<bool>("Error al crear la categoría en la base de datos.");
            }

            return Result.Success(true, "Categoría creada exitosamente.");

        }

        public async Task<Result<bool>> UpdateCategory(UpdateCategoryDto category)
        {

            var existingCategory = await _repository.GetSingleAsync(c => c.Id == category.Id);

            if (existingCategory is null)
            {
                return Result.Failure<bool>("Categoría no encontrada.");
            }

            existingCategory.Name = category.Name;
            existingCategory.DisplayName = category.DisplayName;
            existingCategory.Description = category.Description;
            if (category.Icon is not null)
            {
                var uploadResult = await _storageService.UploadFileAsync(category.Name, category.Icon);
                if (!uploadResult.IsSuccess)
                {
                    return Result.Failure<bool>($"Error al subir la imagen: {uploadResult.Error.Message}");
                }
                existingCategory.Icon = uploadResult.Data!;
            }

            var resultUpdate = await _repository.UpdateAsync(existingCategory);

            if (resultUpdate <= 0)
            {
                return Result.Failure<bool>("Error al actualizar la categoría en la base de datos.");
            }

            return Result.Success(true, "Categoría actualizada exitosamente.");

        }

        public async Task<Result> GetById(Guid id)
        {
            var categoryEntry = await _repository.GetSingleAsync(n => n.Id == id);
            if (categoryEntry is null)
            {
                return Result.Failure("Categoría no encontrada.");
            }
            if (!string.IsNullOrEmpty(categoryEntry.Icon))
            {
                var urlResult = await _storageService.GetUrlFile(categoryEntry.Icon);
                if (urlResult.IsSuccess && urlResult.Data is not null)
                {
                    categoryEntry.Icon = urlResult.Data;
                }
            }
            return Result.Success(categoryEntry, "Categoría obtenida correctamente.");


        }

        public async Task<Result<List<CategoryListVM>>> GetListCategories()
        {

            var categories = await _repository.GetListCategories();

            var imageTasks = categories
               .Where(e => !string.IsNullOrEmpty(e.Icon))
               .Select(async entry =>
               {
                   var urlResult = await _storageService.GetUrlFile(entry.Icon!);
                   if (urlResult.IsSuccess && urlResult.Data is not null)
                   {
                       entry.Icon = urlResult.Data;
                   }
               });

            await Task.WhenAll(imageTasks);

            return Result.Success(categories, "Entradas obtenidas correctamente");

        }

    }
}
