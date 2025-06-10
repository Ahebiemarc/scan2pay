using AutoMapper; // Ajout de IMapper
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using scan2pay.DTOs;
using scan2pay.Interfaces;
using System.Security.Claims;

namespace Scan2Pay.Api.Controllers;

[Authorize] // Toutes les actions ici nécessitent une authentification
[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly ITransactionRepository _transactionRepository; // Pour l'historique
    private readonly IMapper _mapper;

    public WalletsController(IWalletService walletService, ITransactionRepository transactionRepository, IMapper mapper)
    {
        _walletService = walletService;
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    [HttpGet("my-wallet")]
    [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyWallet()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var wallet = await _walletService.GetWalletByUserIdAsync(userId);
        if (wallet == null) return NotFound(new { message = "Portefeuille non trouvé." });
        return Ok(wallet);
    }

    [HttpPost("topup")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TopUp(TopUpRequestDto topUpDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var transaction = await _walletService.TopUpWalletAsync(userId, topUpDto);
        if (transaction == null)
        {
            // Le service devrait logger l'erreur spécifique
            return BadRequest(new { message = "La recharge a échoué. Vérifiez les détails de paiement ou le solde." });
        }
        return Ok(transaction);
    }

    [HttpPost("withdraw")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Withdraw(WithdrawalRequestDto withdrawalDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var transaction = await _walletService.WithdrawFromWalletAsync(userId, withdrawalDto);
         if (transaction == null)
        {
            return BadRequest(new { message = "Le retrait a échoué. Vérifiez le solde ou les détails du compte." });
        }
        return Ok(transaction);
    }

    [HttpGet("my-wallet/transactions")]
    [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyWalletTransactions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "timestamp", // "timestamp", "amount"
        [FromQuery] bool ascending = false)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var wallet = await _walletService.GetWalletByUserIdAsync(userId);
        if (wallet == null) return NotFound(new { message = "Portefeuille non trouvé." });

        // Limiter pageSize pour éviter les abus
        pageSize = Math.Min(pageSize, 50);

        var transactions = await _transactionRepository.GetTransactionsByWalletIdAsync(wallet.Id, pageNumber, pageSize, sortBy, ascending);
        return Ok(_mapper.Map<IEnumerable<TransactionDto>>(transactions));
    }
}