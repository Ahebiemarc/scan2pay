using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scan2pay.Models
{
    public enum TransactionType
    {
        Deposit,      // Recharge
        Withdrawal,   // Retrait
        PaymentSent,  // Paiement effectué
        PaymentReceived, // Paiement reçu
        RefundIssued, // Remboursement émis
        RefundReceived // Remboursement reçu
    }

    public enum TransactionStatus
    {
        Pending,
        Completed,
        Failed,
        Cancelled,
        Processing
    }
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid WalletId { get; set; } // FK
        public virtual Wallet Wallet { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public TransactionStatus Status { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ReferenceId { get; set; } // External ID from payment gateway, or internal ref

        public Guid? RelatedTransactionId { get; set; } // For refunds, points to original transaction
        public virtual Transaction? RelatedTransaction { get; set; }

        public string? PayerId { get; set; } // FK to ApplicationUser
        public virtual ApplicationUser? Payer { get; set; }

        public string? PayeeId { get; set; } // FK to ApplicationUser
        public virtual ApplicationUser? Payee { get; set; }

        public Guid? QrCodeId { get; set; } // If payment was made via QR code
        public virtual QrCode? QrCode { get; set; }

        public Guid? SourcePaymentMethodId { get; set; } // For deposits
        public virtual PaymentMethod? SourcePaymentMethod { get; set; }

        public Guid? DestinationPaymentMethodId { get; set; } // For withdrawals
        public virtual PaymentMethod? DestinationPaymentMethod { get; set; }
    }
}