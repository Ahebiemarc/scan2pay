using scan2pay.DTOs;
using scan2pay.Models;
namespace scan2pay.Interfaces;


public interface IQrCodeService
{
    Task<QrCodeDto?> GenerateUniqueQrCodeForMarchandAsync(ApplicationUser marchand);
    Task<QrCodeDto?> GetMarchandQrCodeAsync(string marchandId);
    Task<QrCodeDto?> GetQrCodeDetailsByDataAsync(string qrCodeData);
    Task<string?> GenerateQrCodeImageDataUrlAsync(string qrCodeData);
}