// File: DTOs/AuthDtos.cs
using System.ComponentModel.DataAnnotations;

namespace scan2pay.DTOs;

public class RegisterUserDto
{
    [Required(ErrorMessage = "Le prénom est requis.")]
    [StringLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom de famille est requis.")]
    [StringLength(100, ErrorMessage = "Le nom de famille ne peut pas dépasser 100 caractères.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est requis.")]
    [EmailAddress(ErrorMessage = "Format d'email invalide.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 100 caractères.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le type d'utilisateur est requis (Client ou Marchand).")]
    public string UserType { get; set; } = string.Empty; // "Client" ou "Marchand"

    [Phone(ErrorMessage = "Format de numéro de téléphone invalide.")]
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }
}

public class LoginUserDto
{
    [Required(ErrorMessage = "L'email est requis.")]
    [EmailAddress(ErrorMessage = "Format d'email invalide.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string UserType { get; set; } = string.Empty;
    public DateTime DateRegistered { get; set; }
    public bool TwoFactorEnabled { get; set; }
}

public class UpdateUserProfileDto
{
    [Required] [StringLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required] [StringLength(100)] public string LastName { get; set; } = string.Empty;
    [Phone] public string? PhoneNumber { get; set; }
    [StringLength(255)] public string? Address { get; set; }
}

public class ChangePasswordDto
{
    [Required] public string CurrentPassword { get; set; } = string.Empty;
    [Required] [StringLength(100, MinimumLength = 8)] public string NewPassword { get; set; } = string.Empty;
    [Required] [Compare("NewPassword")] public string ConfirmNewPassword { get; set; } = string.Empty;
}

public class ForgotPasswordDto
{
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Token { get; set; } = string.Empty; // Token de réinitialisation
    [Required] [StringLength(100, MinimumLength = 8)] public string NewPassword { get; set; } = string.Empty;
    [Required] [Compare("NewPassword")] public string ConfirmNewPassword { get; set; } = string.Empty;
}