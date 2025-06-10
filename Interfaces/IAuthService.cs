// File: Interfaces/IAuthService.cs
using scan2pay.DTOs;
using scan2pay.Models;
using Microsoft.AspNetCore.Identity;

namespace scan2pay.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterUserDto registerDto);
    Task<AuthResponseDto?> LoginAsync(LoginUserDto loginDto);
    Task<IdentityResult> UpdateProfileAsync(string userId, UpdateUserProfileDto profileDto);
    Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
    Task<UserProfileDto?> GetUserProfileAsync(string userId);
    Task<IdentityResult> RequestPasswordResetAsync(string email);
    Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    // Ajoutez 2FA methods
}