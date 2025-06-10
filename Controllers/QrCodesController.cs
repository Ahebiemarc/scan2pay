// File: Controllers/QrCodesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using scan2pay.DTOs;
using scan2pay.Interfaces;
using scan2pay.Models; // Pour UserType
using System.Security.Claims;

namespace scan2pay.Controllers;

[Authorize]
[ApiController]
[Route("api/qrcodes")]
public class QrCodesController : ControllerBase
{
    private readonly IQrCodeService _qrCodeService;

    public QrCodesController(IQrCodeService qrCodeService)
    {
        _qrCodeService = qrCodeService;
    }

    // L'endpoint POST pour créer un QR est supprimé, car il est créé à l'inscription.
    // On pourrait garder un endpoint pour le régénérer si besoin.

    [HttpGet("my-qrcode")]
    [Authorize(Roles = "Marchand")]
    [ProducesResponseType(typeof(QrCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyQrCode()
    {
        var marchandId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(marchandId)) return Unauthorized();

        var qrCode = await _qrCodeService.GetMarchandQrCodeAsync(marchandId);
        if (qrCode == null) return NotFound("QR Code non trouvé pour ce marchand.");

        return Ok(qrCode);
    }
    
    [HttpGet("my-qrcode/image")]
    [Authorize(Roles = "Marchand")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyQrCodeImage()
    {
        var marchandId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(marchandId)) return Unauthorized();

        var qrCodeDetails = await _qrCodeService.GetMarchandQrCodeAsync(marchandId);
        if (qrCodeDetails == null) return NotFound("QR Code non trouvé.");

        var imageDataUrl = await _qrCodeService.GenerateQrCodeImageDataUrlAsync(qrCodeDetails.QrCodeData);
        if (string.IsNullOrEmpty(imageDataUrl))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la génération de l'image du QR code.");
        }
        
        return Ok(new { qrImageBase64 = imageDataUrl });
    }

    // Endpoint public pour que l'app du client puisse valider le marchand avant de payer.
    [HttpGet("validate/{qrCodeData}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(QrCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateQrCode(string qrCodeData)
    {
        if (string.IsNullOrWhiteSpace(qrCodeData)) return BadRequest();

        var qrCode = await _qrCodeService.GetQrCodeDetailsByDataAsync(qrCodeData);
        if (qrCode == null || !qrCode.IsActive) return NotFound("QR Code invalide ou inactif.");

        return Ok(qrCode); // Retourne les détails du QR (sans infos trop sensibles) pour que l'app affiche le nom du marchand.
    }
}