using System.Linq.Expressions;
using EnglishPlatform.Infrastructure.Data;
using EnglishPlatform.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.Infrastructure.Repositories.Implementations;

/// <summary>
/// Generic repository implementation using EF Core.
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public virtual async Task<T?> GetByIdAsync(long id) => await _dbSet.FindAsync(id);

    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.Where(predicate).ToListAsync();

    public virtual IQueryable<T> Query() => _dbSet.AsQueryable();

    public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public virtual async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);

    public virtual void Update(T entity) => _dbSet.Update(entity);

    public virtual void Delete(T entity) => _dbSet.Remove(entity);

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.AnyAsync(predicate);

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null) =>
        predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
}
