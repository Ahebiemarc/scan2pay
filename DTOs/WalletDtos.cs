// File: DTOs/WalletDtos.cs
using System.ComponentModel.DataAnnotations;

namespace scan2pay.DTOs;

public class WalletDto
{
    public Guid Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}

public class TopUpRequestDto
{
    [Required]
    [Range(0.01, 10000, ErrorMessage = "Le montant doit être entre 0.01 et 10000.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "L'ID de la méthode de paiement est requis.")]
    public Guid PaymentMethodId { get; set; } // ID de la carte/compte enregistré par l'utilisateur

    [StringLength(3)]
    public string Currency { get; set; } = "EUR";
}

public class WithdrawalRequestDto
{
    [Required]
    [Range(0.01, 10000, ErrorMessage = "Le montant doit être entre 0.01 et 10000.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "L'ID de la méthode de paiement de destination est requis.")]
    public Guid DestinationPaymentMethodId { get; set; } // ID du compte bancaire enregistré

    [StringLength(3)]
    public string Currency { get; set; } = "EUR";

    [StringLength(200)]
    public string? Description { get; set; }
}
