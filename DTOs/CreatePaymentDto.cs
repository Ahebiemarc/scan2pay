using System.ComponentModel.DataAnnotations;

namespace scan2pay.DTOs;

public class CreatePaymentDto
{
    [Required]
    public string QrCodeData { get; set; } = string.Empty; // Données du QR code scanné

    [Required]
    [Range(0.01, 10000, ErrorMessage = "Le montant à payer est requis et doit être valide.")]
    public decimal Amount { get; set; } // Montant est maintenant requis, car saisi par le client
}
