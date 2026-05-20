using Microsoft.EntityFrameworkCore;
using PMS.Application.DTOs.Pagination;
using PMS.Application.Interfaces.Persistence;
using PMS.Infrastructure.Data;
using System.Linq.Expressions;

namespace PMS.Infrastructure.Services.Persistence;

public class Repository<T> : IRepository<T> where T : class
{

    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _dbSet = context.Set<T>();
    }

    public async Task<PagedResult<TDto>> GetPagedAsync<TDto>(
            IQueryable<T> query,
            int pageNumber,
            int pageSize,
            Expression<Func<T, TDto>> mapper,
            string? orderBy = null,
            bool isDescending = false,
            CancellationToken cancellationToken = default)
    {

        if (!string.IsNullOrWhiteSpace(orderBy) && IsValidProperty(orderBy))
        {
            query = isDescending
                ? query.OrderByDescending(x => EF.Property<int>(x, orderBy))
                : query.OrderBy(x => EF.Property<int>(x, orderBy));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .Select(mapper)
                               .ToListAsync(cancellationToken);

        return new PagedResult<TDto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => await _dbSet.FindAsync(id, cancellationToken);

    public IQueryable<T> GetQueryable(Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return query;
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) => await _dbSet.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default) => await _dbSet.AddRangeAsync(entities, cancellationToken);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public void DeleteRange(List<T> entities) => _dbSet.RemoveRange(entities);

    private bool IsValidProperty(string propertyName)
    {
        return typeof(T).GetProperties()
            .Any(p => p.Name.Equals(propertyName));
    }
}
