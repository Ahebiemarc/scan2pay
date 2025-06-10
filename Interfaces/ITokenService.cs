// File: Interfaces/ITokenService.cs
using scan2pay.Models;
using System.Security.Claims;

namespace scan2pay.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(ApplicationUser user, IList<string> roles);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    // string GenerateRefreshToken(); // Si vous implémentez les refresh tokens
}
