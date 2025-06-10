// File: Interfaces/IWalletRepository.cs
using scan2pay.Models;
namespace scan2pay.Interfaces;

public interface IWalletRepository : IGenericRepository<Wallet>
{
    Task<Wallet?> GetByUserIdAsync(string userId);
}