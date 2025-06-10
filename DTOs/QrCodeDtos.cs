// File: DTOs/QrCodeDtos.cs
using System.ComponentModel.DataAnnotations;

namespace scan2pay.DTOs;



public class QrCodeDto
{
    public Guid Id { get; set; }
    public string MarchandId { get; set; } = string.Empty;
    public string QrCodeData { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UsageCount { get; set; }
}

/*public class CreateQrCodeDto // Pour le marchand
{
    [Range(0.01, 100000, ErrorMessage = "Le montant doit être positif ou nul (pour QR générique).")]
    public decimal? Amount { get; set; } // Optionnel, si QR pour montant spécifique

    [StringLength(255)]
    public string? Description { get; set; }
}*/