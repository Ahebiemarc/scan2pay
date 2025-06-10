// File: Controllers/PaymentsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using scan2pay.DTOs;
using scan2pay.Interfaces;
using System.Security.Claims;

namespace scan2pay.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost("qr")]
    [Authorize(Roles = "Client")] // Seul un client peut initier un paiement par QR
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)] // Solde insuffisant
    public async Task<IActionResult> ProcessQrPayment(CreatePaymentDto paymentDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var payingUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(payingUserId)) return Unauthorized();

        var transactionResult = await _paymentService.ProcessQrPaymentAsync(payingUserId, paymentDto);

        if (transactionResult == null)
        {
            // Les raisons spécifiques (QR invalide, marchand non trouvé, etc.) sont loggées par le service
            return BadRequest(new { message = "Le paiement a échoué. Veuillez vérifier les détails du QR code ou votre solde." });
        }

        if (transactionResult.Status == "Failed") // Vérifier si c'est un échec de solde
        {
             return StatusCode(StatusCodes.Status402PaymentRequired, transactionResult);
        }

        return Ok(transactionResult);
    }
}