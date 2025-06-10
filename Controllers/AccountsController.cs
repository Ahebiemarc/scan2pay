// File: Controllers/AccountsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using scan2pay.DTOs;
using scan2pay.Interfaces;
using System.Security.Claims;

namespace Scan2Pay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IAuthService authService, ILogger<AccountsController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterUserDto registerDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(registerDto);
        if (result == null)
        {
            return BadRequest(new { message = "L'enregistrement a échoué. L'email est peut-être déjà utilisé ou le mot de passe n'est pas conforme." });
        }
        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginUserDto loginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.LoginAsync(loginDto);
        if (result == null)
        {
            return Unauthorized(new { message = "Email ou mot de passe incorrect." });
        }
        return Ok(result);
    }

    [HttpGet("profile")]
    [Authorize] // Seuls les utilisateurs connectés peuvent accéder
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var profile = await _authService.GetUserProfileAsync(userId);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile(UpdateUserProfileDto profileDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _authService.UpdateProfileAsync(userId, profileDto);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return NoContent();
    }

    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
         if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return NoContent();
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _authService.RequestPasswordResetAsync(forgotPasswordDto.Email);
        // Toujours retourner OK pour ne pas révéler si un email existe
        return Ok(new { message = "Si votre email est enregistré, vous recevrez des instructions pour réinitialiser votre mot de passe." });
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.ResetPasswordAsync(resetPasswordDto);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok(new { message = "Votre mot de passe a été réinitialisé avec succès." });
    }
}