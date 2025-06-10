// File: Interfaces/IPaymentMethodRepository.cs
using scan2pay.Models;
namespace scan2pay.Interfaces;


public interface IPaymentMethodRepository : IGenericRepository<PaymentMethod>
{
    Task<IEnumerable<PaymentMethod>> GetPaymentMethodsByUserIdAsync(string userId);
    Task<PaymentMethod?> GetDefaultPaymentMethodAsync(string userId);
}