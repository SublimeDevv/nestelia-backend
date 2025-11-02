using AutoMapper;
using Nestelia.Application.Extensions;
using Nestelia.Application.Interfaces.Base;
using Nestelia.Domain.DTO;
using Nestelia.Domain.Shared;
using Nestelia.Infraestructure.Interfaces.Generic;
using System.Linq.Expressions;

namespace Nestelia.Application.Services.Base
{
    public class ServiceBase<T, TDto>(IMapper mapper, IBaseRepository<T> baseRepository) : IServiceBase<T, TDto> where T : class where TDto : BaseDto
    {
        private readonly IMapper _mapper = mapper;
        private readonly IBaseRepository<T> _repository = baseRepository;

        public async Task<Result> GetAllAsync(int page = 1, int size = 10, Expression<Func<T, bool>>? filter = null)
        {
            page = Math.Max(1, page);
            size = Math.Clamp(size, 1, 100);

            var pagedData = await _repository.GetAllAsync(page, size, filter);

            return Result.Success(pagedData.Items, "Lista obtenida correctamente").With("pagination", new { currentPage = page, pageSize = size, totalPages = pagedData.TotalPages, totalCount = pagedData.TotalCount });
        }

        public virtual async Task<Result> InsertAsync(T entity)
        {

            var result = await _repository.InsertAsync(entity);
            if (result == Guid.Empty)
            {
                return Result.Failure($"No se pudo insertar el elemento de tipo {typeof(T).GetDisplayName()}.");
            }

            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null && idProperty.CanWrite)
            {
                idProperty.SetValue(entity, result, null);
            }

            return Result.Success(entity, "El elemento fue insertado correctamente");

        }

        public async Task<Result> UpdateAsync(T entity)
        {

            var result = await _repository.UpdateAsync(entity);
            if (result <= 0)
            {
                return Result.Failure($"No se pudo actualizar el elemento de tipo {typeof(T).GetDisplayName()}.");
            }

            return Result.Success(entity, $"El elemento {typeof(T).GetDisplayName()} fue actualizado correctamente");
        }

        public async Task<Result> RemoveAsync(T entity)
        {
            var result = await _repository.RemoveAsync(entity);
            if (result <= 0)
            {
                return Result.Failure($"No se pudo eliminar el elemento de tipo {typeof(T).GetDisplayName()}.");
            }

            return Result.Success(entity, $"El elemento {typeof(T).GetDisplayName()} fue eliminado correctamente");

        }

        public async Task<Result> RemoveAsync(Guid id)
        {

            var result = await _repository.RemoveAsync(id);
            if (result <= 0)
            {
                return Result.Failure($"No se pudo eliminar el elemento de tipo {typeof(T).GetDisplayName()}.");
            }

            return Result.Success(id, $"El elemento {typeof(T).GetDisplayName()} fue eliminado correctamente");
        }

        public async Task<Result<T>> GetById(Expression<Func<T, bool>>? filter = null)
        {
            var data = await _repository.GetSingleAsync(filter);
            if (data is null)
            {
                return Result.Failure<T>($"No se encontró el elemento de tipo {typeof(T).GetDisplayName()}.");
            }

            return Result.Success(data, "Elemento obtenido correctamente");

        }

        public async Task<TDto> ConvertToDto(T entity)
        {
            return await Task.FromResult(_mapper.Map<TDto>(entity));
        }
        public async Task<T> ConvertToEntity(TDto dto)
        {
            return await Task.FromResult(_mapper.Map<T>(dto));
        }
        public async Task<List<TDto>> ConvertToDto(List<T> entities)
        {
            return await Task.FromResult(_mapper.Map<List<TDto>>(entities));
        }
        public async Task<List<T>> ConvertToEntity(List<TDto> dto)
        {
            return await Task.FromResult(_mapper.Map<List<T>>(dto));
        }
    }
}
