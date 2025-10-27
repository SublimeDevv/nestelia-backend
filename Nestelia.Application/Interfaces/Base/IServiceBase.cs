using Nestelia.Domain.DTO;
using System.Linq.Expressions;
using Nestelia.Domain.Shared;

namespace Nestelia.Application.Interfaces.Base
{
    public interface IServiceBase<T, TDto> where T : class where TDto : BaseDto
    {
        Task<Result> GetAllAsync(int page = 1, int size = 10, Expression<Func<T, bool>>? filter = null);
        Task<Result> InsertAsync(T entity);
        Task<Result> UpdateAsync(T entity);
        Task<Result<T>> GetById(Expression<Func<T, bool>>? filter = null);
        Task<Result> RemoveAsync(T entity);
        Task<Result> RemoveAsync(Guid Id);
        Task<TDto> ConvertToDto(T entity);
        Task<T> ConvertToEntity(TDto dto);
        Task<List<TDto>> ConvertToDto(List<T> entity);
        Task<List<T>> ConvertToEntity(List<TDto> dto);
    }
}
