// File: DTOs/PaymentMethodDtos.cs
using System.ComponentModel.DataAnnotations;

namespace scan2pay.DTOs;

public class PaymentMethodDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty; // Card, BankAccount, etc.
    public string Provider { get; set; } = string.Empty;
    public string MaskedIdentifier { get; set; } = string.Empty;
    public string? ExpiryDate { get; set; }
    public bool IsDefault { get; set; }
    public DateTime AddedAt { get; set; }
}

public class CreatePaymentMethodDto
{
    [Required]
    public string Type { get; set; } = string.Empty; // "Card", "BankAccount", "MobileMoney", "PayPal"

    [Required]
    public string Provider { get; set; } = string.Empty; // "Visa", "Stripe", "OrangeMoney"

    // Ces champs seraient remplacés par un token de la gateway (ex: Stripe token, PayPal order ID)
    // Pour la simulation, on peut les garder simples.
    [Required]
    public string TokenFromGateway { get; set; } = string.Empty; // e.g., Stripe PaymentMethod ID, PayPal authorization_id

    public bool IsDefault { get; set; } = false;
}