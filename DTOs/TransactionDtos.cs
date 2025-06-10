// File: DTOs/TransactionDtos.cs
using System.ComponentModel.DataAnnotations;

namespace scan2pay.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Utiliser string pour TransactionType.ToString()
    public string Status { get; set; } = string.Empty; // Utiliser string pour TransactionStatus.ToString()
    public DateTime Timestamp { get; set; }
    public string? Description { get; set; }
    public string? ReferenceId { get; set; }
    public Guid? RelatedTransactionId { get; set; }
    public string? PayerEmail { get; set; } // Email au lieu de ID pour affichage
    public string? PayeeEmail { get; set; } // Email au lieu de ID pour affichage
    public Guid? QrCodeId { get; set; }
}

