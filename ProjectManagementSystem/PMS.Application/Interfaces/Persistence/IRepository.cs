using PMS.Application.DTOs.Pagination;
using System.Linq.Expressions;

namespace PMS.Application.Interfaces.Persistence;

public interface IRepository<T> where T : class
{
    Task<PagedResult<TDto>> GetPagedAsync<TDto>(
            IQueryable<T> query,
            int pageNumber,
            int pageSize,
            Expression<Func<T, TDto>> mapper,
            string? orderBy = null,
            bool isDescending = false,
            CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    IQueryable<T> GetQueryable(Expression<Func<T, bool>>? filter = null);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default);

    void Delete(T entity);

    void DeleteRange(List<T> entities);
}
