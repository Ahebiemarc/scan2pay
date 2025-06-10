// File: Interfaces/ITransactionRepository.cs
using scan2pay.Models;
namespace scan2pay.Interfaces;


public interface ITransactionRepository : IGenericRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetTransactionsByWalletIdAsync(Guid walletId, int pageNumber, int pageSize, string? sortBy, bool ascending);
}