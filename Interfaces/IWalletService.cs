// File: Interfaces/IWalletService.cs
using scan2pay.DTOs;
using scan2pay.Models;
namespace scan2pay.Interfaces;
public interface IWalletService
{
    Task<WalletDto?> GetWalletByUserIdAsync(string userId);
    Task<TransactionDto?> TopUpWalletAsync(string userId, TopUpRequestDto topUpRequestDto);
    Task<TransactionDto?> WithdrawFromWalletAsync(string userId, WithdrawalRequestDto withdrawalRequestDto);
    Task<bool> CreateWalletForUserAsync(ApplicationUser user);
}