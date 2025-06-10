// File: Repositories/PaymentMethodRepository.cs
using Microsoft.EntityFrameworkCore;
using scan2pay.Data;
using scan2pay.Models;
using scan2pay.Interfaces;
namespace scan2pay.Repositories;
public class PaymentMethodRepository : GenericRepository<PaymentMethod>, IPaymentMethodRepository
{
    public PaymentMethodRepository(ApplicationDbContext context) : base(context) { }
    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsByUserIdAsync(string userId) =>
        await _dbSet.Where(pm => pm.ApplicationUserId == userId).ToListAsync();
    public async Task<PaymentMethod?> GetDefaultPaymentMethodAsync(string userId) =>
        await _dbSet.FirstOrDefaultAsync(pm => pm.ApplicationUserId == userId && pm.IsDefault);
}