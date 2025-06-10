using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace scan2pay.Models
{
    public enum NotificationType
    {
        PaymentSuccess, // pour le payeur
        PaymentFailed,
        RefundProcessed,
        NewLoginAlert,
        BalanceTopUp,
        Promotional,
        SecurityAlert,
        PaymentReceived,   //  Pour le bénéficiaire/marchand
        NewOrderReceived  // Optionnel: plus spécifique pour les commandes e-commerce

    }
    public class Notification
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string ApplicationUserId { get; set; } = string.Empty; // FK
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public NotificationType Type { get; set; }
        public string? RelatedEntityId { get; set; } // e.g., TransactionId
    }
}