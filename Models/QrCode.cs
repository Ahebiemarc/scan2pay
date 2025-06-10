using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace scan2pay.Models
{
    public class QrCode
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string MarchandId { get; set; } = string.Empty; // FK to ApplicationUser (Marchand)
        public virtual ApplicationUser Marchand { get; set; } = null!;

        [Required]
        public string QrCodeData { get; set; } = string.Empty; // Data to be encoded (e.g., URL, JSON)

        // Un montant fixe n'est plus nécessaire ici, il est saisi par le client
        // [Column(TypeName = "decimal(18, 2)")]
        // public decimal? Amount { get; set; } 

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UsageCount { get; set; } = 0; // How many times it has been scanned/used

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}