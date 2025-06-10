// File: Interfaces/IPaymentService.cs
using scan2pay.DTOs;
using scan2pay.Models;
namespace scan2pay.Interfaces;


public interface IPaymentService
{
    Task<TransactionDto?> ProcessQrPaymentAsync(string payingUserId, CreatePaymentDto paymentDto);
}