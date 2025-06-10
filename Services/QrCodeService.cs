// File: Services/QrCodeService.cs
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QRCoder; // Importer QRCoder
using scan2pay.DTOs;
using scan2pay.Interfaces;
using scan2pay.Models;
using System.Drawing; // Pour Bitmap (nécessite System.Drawing.Common)
using System.Drawing.Imaging; // Pour ImageFormat
using System.IO; // Pour MemoryStream

namespace scan2pay.Services;

public class QrCodeService : IQrCodeService
{
    private readonly IQrCodeRepository _qrCodeRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<QrCodeService> _logger;

    public QrCodeService(
        IQrCodeRepository qrCodeRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<QrCodeService> logger)
    {
        _qrCodeRepository = qrCodeRepository;
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<QrCodeDto?> GenerateUniqueQrCodeForMarchandAsync(ApplicationUser marchand)
    {
        if (marchand.UserType != UserType.Marchand) return null;

        var existingQr = await _qrCodeRepository.GetByMarchandIdAsync(marchand.Id);
        if(existingQr != null) {
            _logger.LogInformation($"QR Code already exists for marchand {marchand.Id}.");
            return _mapper.Map<QrCodeDto>(existingQr);
        }

        // Le QR code contient maintenant l'ID du marchand, qui est une donnée publique et stable.
        string qrDataPayload = $"SCAN2PAY_MARCHANDID_{marchand.Id}";

        var qrCode = new QrCode
        {
            MarchandId = marchand.Id,
            QrCodeData = qrDataPayload,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _qrCodeRepository.AddAsync(qrCode);
        if (await _qrCodeRepository.SaveChangesAsync() > 0)
        {
            return _mapper.Map<QrCodeDto>(qrCode);
        }
        return null;
    }
    
    public async Task<QrCodeDto?> GetMarchandQrCodeAsync(string marchandId)
    {
        var qrCode = await _qrCodeRepository.GetByMarchandIdAsync(marchandId);
        return _mapper.Map<QrCodeDto>(qrCode);
    }

    public Task<string?> GenerateQrCodeImageDataUrlAsync(string qrCodeData)
    {
        try
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeDataObject = qrGenerator.CreateQrCode(qrCodeData, QRCodeGenerator.ECCLevel.Q);
            
            // Utiliser PngByteQRCode pour obtenir les bytes directement
            PngByteQRCode pngQrCode = new PngByteQRCode(qrCodeDataObject);
            byte[] qrCodeBytes = pngQrCode.GetGraphic(20); // 20 pixels per module

            return Task.FromResult($"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}")!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating QR code image for data: {qrCodeData}");
            return Task.FromResult<string?>(null);
        }
    }


    public async Task<IEnumerable<QrCodeDto>> GetMarchandQrCodesAsync(string marchandId)
    {
        var qrCodes = await _qrCodeRepository.GetByMarchandIdAsync(marchandId);
        return _mapper.Map<IEnumerable<QrCodeDto>>(qrCodes);
    }

    public async Task<QrCodeDto?> GetQrCodeDetailsAsync(Guid qrCodeId)
    {
        var qrCode = await _qrCodeRepository.GetByIdAsync(qrCodeId);
        return _mapper.Map<QrCodeDto>(qrCode);
    }
    public async Task<QrCodeDto?> GetQrCodeDetailsByDataAsync(string qrCodeData)
    {
        var qrCode = await _qrCodeRepository.GetByQrCodeDataAsync(qrCodeData);
        return _mapper.Map<QrCodeDto>(qrCode);
    }


    public async Task<bool> DeactivateQrCodeAsync(Guid qrCodeId, string marchandId)
    {
        var qrCode = await _qrCodeRepository.GetByIdAsync(qrCodeId);
        if (qrCode == null || qrCode.MarchandId != marchandId)
        {
            _logger.LogWarning($"QR code {qrCodeId} not found or does not belong to marchand {marchandId}.");
            return false;
        }

        qrCode.IsActive = false;
        _qrCodeRepository.Update(qrCode);
        return await _qrCodeRepository.SaveChangesAsync() > 0;
    }
}