// File: Services/WalletService.cs
using AutoMapper;
using scan2pay.DTOs;
using scan2pay.Interfaces;
using scan2pay.Models;
using Microsoft.Extensions.Logging;

namespace scan2pay.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository; // Pour enregistrer les transactions de recharge/retrait
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<WalletService> _logger;

    public WalletService(
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        IPaymentMethodRepository paymentMethodRepository,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<WalletService> logger)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

     public async Task<bool> CreateWalletForUserAsync(ApplicationUser user)
    {
        var existingWallet = await _walletRepository.GetByUserIdAsync(user.Id);
        if (existingWallet != null)
        {
            _logger.LogInformation($"Wallet already exists for user {user.Id}.");
            return true; // Ou false si c'est une erreur
        }

        var wallet = new Wallet
        {
            ApplicationUserId = user.Id,
            Balance = 0, // Solde initial
            Currency = "EUR", // Devise par défaut
            LastUpdated = DateTime.UtcNow
        };

        await _walletRepository.AddAsync(wallet);
        var saved = await _walletRepository.SaveChangesAsync() > 0;
        if(saved)
        {
            user.WalletId = wallet.Id; // Mettre à jour l'utilisateur avec l'ID du portefeuille
            // Note: UserManager.UpdateAsync(user) serait nécessaire si ApplicationUser est modifié ici
            // et que ce service n'a pas UserManager. Il vaut mieux que AuthService gère la mise à jour de User.
        }
        return saved;
    }


    public async Task<WalletDto?> GetWalletByUserIdAsync(string userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        return _mapper.Map<WalletDto>(wallet);
    }

    public async Task<TransactionDto?> TopUpWalletAsync(string userId, TopUpRequestDto topUpRequestDto)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null)
        {
            _logger.LogWarning($"Wallet not found for user {userId} during top-up.");
            return null; // Ou lever une exception
        }

        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(topUpRequestDto.PaymentMethodId);
        if (paymentMethod == null || paymentMethod.ApplicationUserId != userId)
        {
             _logger.LogWarning($"Payment method {topUpRequestDto.PaymentMethodId} not found or doesn't belong to user {userId}.");
            return null;
        }

        // --- Simulation de l'interaction avec la passerelle de paiement ---
        _logger.LogInformation($"Simulating payment gateway interaction for top-up: User {userId}, Amount {topUpRequestDto.Amount}, Method {paymentMethod.Provider}");
        bool paymentSuccessful = true; // Simuler succès
        string externalTransactionId = $"SIM_TOPUP_{Guid.NewGuid().ToString().Substring(0, 8)}";

        if (!paymentSuccessful)
        {
            // Enregistrer la transaction échouée
            var failedTx = new Transaction { /* ... détails ... */ Status = TransactionStatus.Failed };
            await _transactionRepository.AddAsync(failedTx);
            await _transactionRepository.SaveChangesAsync();
            return _mapper.Map<TransactionDto>(failedTx);
        }
        // --- Fin de la simulation ---

        wallet.Balance += topUpRequestDto.Amount;
        wallet.LastUpdated = DateTime.UtcNow;
        _walletRepository.Update(wallet);

        var transaction = new Transaction
        {
            WalletId = wallet.Id,
            Amount = topUpRequestDto.Amount,
            Currency = topUpRequestDto.Currency,
            Type = TransactionType.Deposit,
            Status = TransactionStatus.Completed,
            Timestamp = DateTime.UtcNow,
            Description = $"Recharge via {paymentMethod.Provider} ({paymentMethod.MaskedIdentifier})",
            ReferenceId = externalTransactionId, // ID de la transaction de la passerelle
            SourcePaymentMethodId = paymentMethod.Id
        };
        await _transactionRepository.AddAsync(transaction);

        if (await _walletRepository.SaveChangesAsync() > 0)
        {
            await _notificationService.CreateNotificationAsync(userId, $"Votre portefeuille a été rechargé de {transaction.Amount} {transaction.Currency}.", NotificationType.BalanceTopUp, transaction.Id.ToString());
            return _mapper.Map<TransactionDto>(transaction);
        }
        return null;
    }

    public async Task<TransactionDto?> WithdrawFromWalletAsync(string userId, WithdrawalRequestDto withdrawalRequestDto)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null) return null;

        if (wallet.Balance < withdrawalRequestDto.Amount)
        {
            _logger.LogWarning($"Insufficient balance for user {userId} for withdrawal of {withdrawalRequestDto.Amount}.");
            // Peut-être créer une transaction échouée ici aussi
            return null; // Solde insuffisant
        }

        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(withdrawalRequestDto.DestinationPaymentMethodId);
         if (paymentMethod == null || paymentMethod.ApplicationUserId != userId || paymentMethod.Type != PaymentMethodType.BankAccount) // S'assurer que c'est un compte bancaire
        {
             _logger.LogWarning($"Destination payment method {withdrawalRequestDto.DestinationPaymentMethodId} invalid for user {userId}.");
            return null;
        }

        // --- Simulation du transfert vers la banque ---
        _logger.LogInformation($"Simulating bank transfer for withdrawal: User {userId}, Amount {withdrawalRequestDto.Amount}, To {paymentMethod.MaskedIdentifier}");
        bool transferSuccessful = true; // Simuler succès
        string externalTransferId = $"SIM_WITHDRAW_{Guid.NewGuid().ToString().Substring(0, 8)}";

        if (!transferSuccessful)
        {
            // Enregistrer la transaction échouée
            var failedTx = new Transaction { /* ... détails ... */ Status = TransactionStatus.Failed };
            // ...
            return _mapper.Map<TransactionDto>(failedTx);
        }
        // --- Fin de la simulation ---

        wallet.Balance -= withdrawalRequestDto.Amount;
        wallet.LastUpdated = DateTime.UtcNow;
        _walletRepository.Update(wallet);

        var transaction = new Transaction
        {
            WalletId = wallet.Id,
            Amount = withdrawalRequestDto.Amount, // Montant positif, car c'est le montant de la transaction
            Currency = withdrawalRequestDto.Currency,
            Type = TransactionType.Withdrawal,
            Status = TransactionStatus.Processing, // Ou Completed si instantané
            Timestamp = DateTime.UtcNow,
            Description = withdrawalRequestDto.Description ?? $"Retrait vers {paymentMethod.Provider} ({paymentMethod.MaskedIdentifier})",
            ReferenceId = externalTransferId,
            DestinationPaymentMethodId = paymentMethod.Id
        };
        await _transactionRepository.AddAsync(transaction);

        if (await _walletRepository.SaveChangesAsync() > 0)
        {
            await _notificationService.CreateNotificationAsync(userId, $"Un retrait de {transaction.Amount} {transaction.Currency} est en cours de traitement.", NotificationType.SecurityAlert, transaction.Id.ToString());
            return _mapper.Map<TransactionDto>(transaction);
        }
        return null;
    }
}