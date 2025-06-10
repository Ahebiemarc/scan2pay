using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace scan2pay.Models
{
    public class Wallet
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string ApplicationUserId { get; set; } = string.Empty; // FK
    public virtual ApplicationUser ApplicationUser { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; } = 0.00m;

    [Required]
    [StringLength(3)] // e.g., EUR, USD
    public string Currency { get; set; } = "EUR";

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
}