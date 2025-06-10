// File: Repositories/GenericRepository.cs
using Microsoft.EntityFrameworkCore;
using scan2pay.Data;
using scan2pay.Interfaces;
using System.Linq.Expressions;

namespace scan2pay.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);
    public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public virtual void Update(T entity) => _dbSet.Update(entity);
    public virtual void Delete(T entity) => _dbSet.Remove(entity);
    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}