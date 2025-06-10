// File: Repositories/WalletRepository.cs
using Microsoft.EntityFrameworkCore;
using scan2pay.Data;
using scan2pay.Models;
using scan2pay.Interfaces;

namespace scan2pay.Repositories;

public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
{
    public WalletRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Wallet?> GetByUserIdAsync(string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(w => w.ApplicationUserId == userId);
    }
}