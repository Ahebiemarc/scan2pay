// File: Services/PaymentService.cs
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using scan2pay.Data;
using scan2pay.DTOs;
using scan2pay.Interfaces;
using scan2pay.Models;
using System.Threading.Tasks; // Assurez-vous que ce using est présent

namespace scan2pay.Services;

public class PaymentService : IPaymentService
{
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IQrCodeRepository _qrCodeRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<PaymentService> _logger;
    private readonly ApplicationDbContext _context;

    public PaymentService(
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        IQrCodeRepository qrCodeRepository,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<PaymentService> logger,
        ApplicationDbContext context)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _qrCodeRepository = qrCodeRepository;
        _userManager = userManager;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
        _context = context;
    }

    public async Task<TransactionDto?> ProcessQrPaymentAsync(string payingUserId, CreatePaymentDto paymentDto)
    {
        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var qrCodeDetails = await _qrCodeRepository.GetByQrCodeDataAsync(paymentDto.QrCodeData);
            if (qrCodeDetails == null || !qrCodeDetails.IsActive)
            {
                _logger.LogWarning($"QR Code invalide ou inactif: {paymentDto.QrCodeData}.");
                await dbTransaction.RollbackAsync();
                return null;
            }

            var payer = await _userManager.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == payingUserId);
            var marchand = await _userManager.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == qrCodeDetails.MarchandId);

            if (payer?.Wallet == null || marchand?.Wallet == null)
            {
                _logger.LogError("Payer ou Marchand (ou leurs portefeuilles) non trouvé.");
                await dbTransaction.RollbackAsync();
                return null;
            }

            decimal amountToPay = paymentDto.Amount; // Le montant est maintenant obligatoire et fourni par le client.
            if (payer.Wallet.Balance < amountToPay)
            {
                // ... gestion solde insuffisant
                return null;
            }

            // Débiter le payeur, créditer le marchand, etc.
            payer.Wallet.Balance -= amountToPay;
            marchand.Wallet.Balance += amountToPay;
            qrCodeDetails.UsageCount++;

            _walletRepository.Update(payer.Wallet);
            _walletRepository.Update(marchand.Wallet);
            _qrCodeRepository.Update(qrCodeDetails);

            // Créer les deux transactions (PaymentSent et PaymentReceived)
            var payerTransaction = new Transaction
            {
                WalletId = payer.Wallet.Id, Amount = amountToPay, Type = TransactionType.PaymentSent,
                Status = TransactionStatus.Completed, Description = $"Paiement à {marchand.FirstName}",
                PayerId = payingUserId, PayeeId = marchand.Id, QrCodeId = qrCodeDetails.Id
            };
            await _transactionRepository.AddAsync(payerTransaction);

            var marchandTransaction = new Transaction
            {
                WalletId = marchand.Wallet.Id, Amount = amountToPay, Type = TransactionType.PaymentReceived,
                Status = TransactionStatus.Completed, Description = $"Paiement reçu de {payer.FirstName}",
                PayerId = payingUserId, PayeeId = marchand.Id, QrCodeId = qrCodeDetails.Id,
                RelatedTransactionId = payerTransaction.Id
            };
            await _transactionRepository.AddAsync(marchandTransaction);

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            await _notificationService.CreateNotificationAsync(payingUserId, $"Paiement de {amountToPay:C} effectué.", NotificationType.PaymentSuccess, payerTransaction.Id.ToString());
            await _notificationService.CreateNotificationAsync(marchand.Id, $"Paiement de {amountToPay:C} reçu.", NotificationType.PaymentReceived, marchandTransaction.Id.ToString());

            return _mapper.Map<TransactionDto>(payerTransaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception lors du traitement du paiement par QR.");
            await dbTransaction.RollbackAsync();
            return null;
        }
    }
}