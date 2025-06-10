using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace scan2pay.Models
{
    public enum PaymentMethodType
    {
        Card,
        BankAccount,
        MobileMoney, // For Orange Money, etc.
        PayPal // Specific for PayPal
    }
    public class PaymentMethod
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty; // FK
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        public PaymentMethodType Type { get; set; }

        [Required]
        [StringLength(50)]
        public string Provider { get; set; } = string.Empty; // e.g., "Visa", "Mastercard", "PayPal", "Stripe", "OrangeMoney"

        [Required]
        [StringLength(50)]
        public string MaskedIdentifier { get; set; } = string.Empty; // e.g., "•••• 1234" or masked email

        [StringLength(7)] // MM/YYYY
        public string? ExpiryDate { get; set; } // For cards

        public bool IsDefault { get; set; } = false;

        [StringLength(255)]
        public string? ExternalToken { get; set; } // Token from Stripe, PayPal, etc.

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Transaction> DepositTransactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Transaction> WithdrawalTransactions { get; set; } = new List<Transaction>();
    }
}