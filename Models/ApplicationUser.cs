using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace scan2pay.Models
{
    public enum UserType
    {
        Client,
        Marchand
    }
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Address { get; set; }

        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;

        [Required]
        public UserType UserType { get; set; }

        // Navigation Properties
        public Guid? WalletId { get; set; } // Nullable si le portefeuille est créé séparément
        public virtual Wallet? Wallet { get; set; }

        // Un marchand a un seul QR code unique
        public virtual QrCode? QrCode { get; set; }

        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
        public virtual ICollection<Transaction> InitiatedTransactions { get; set; } = new List<Transaction>(); // Transactions where this user is payer
        public virtual ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>(); // Transactions where this user is payee
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}