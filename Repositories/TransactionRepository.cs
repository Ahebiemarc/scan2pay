// File: Repositories/TransactionRepository.cs
using Microsoft.EntityFrameworkCore;
using scan2pay.Data;
using scan2pay.Models;
using scan2pay.Interfaces;
using System.Linq;

namespace scan2pay.Repositories;

public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Transaction>> GetTransactionsByWalletIdAsync(Guid walletId, int pageNumber, int pageSize, string? sortBy, bool ascending)
    {
        IQueryable<Transaction> query = _dbSet.Where(t => t.WalletId == walletId)
                          .Include(t => t.Payer) // Inclure l'utilisateur payeur
                          .Include(t => t.Payee); // Inclure l'utilisateur bénéficiaire

        // Tri
        if (!string.IsNullOrEmpty(sortBy))
        {
            // Exemple simple de tri, à étendre pour plus de champs
            query = sortBy.ToLowerInvariant() switch
            {
                "amount" => ascending ? query.OrderBy(t => t.Amount) : query.OrderByDescending(t => t.Amount),
                "timestamp" => ascending ? query.OrderBy(t => t.Timestamp) : query.OrderByDescending(t => t.Timestamp),
                _ => query.OrderByDescending(t => t.Timestamp) // Tri par défaut
            };
        }
        else
        {
            query = query.OrderByDescending(t => t.Timestamp); // Tri par défaut
        }

        // Pagination
        return await query.Skip((pageNumber - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();
    }
}
// Implémentez les autres repositories (QrCodeRepository, PaymentMethodRepository, etc.) sur le même modèle...
