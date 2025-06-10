// File: Interfaces/IGenericRepository.cs (Optionnel, pour CRUD de base)
namespace scan2pay.Interfaces;

/*public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetByIdAsync(int id); // Pour les entités avec int PK
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity); // EF Core suit les changements
    void Delete(T entity);
    Task<int> SaveChangesAsync();
    // Ajoutez des méthodes pour la pagination/filtrage/tri si générique
}*/


public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync();
}