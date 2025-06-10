// File: Interfaces/IQrCodeRepository.cs
using scan2pay.Models;
namespace scan2pay.Interfaces;



public interface IQrCodeRepository : IGenericRepository<QrCode>
{
    Task<QrCode?> GetByMarchandIdAsync(string marchandId); // Changé pour récupérer le QR unique
    Task<QrCode?> GetByQrCodeDataAsync(string qrCodeData);
}