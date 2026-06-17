using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CesiZen.API.DTOs.Auth;
using CesiZen.API.Services.Interfaces;

namespace CesiZen.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Inscription d'un nouvel utilisateur</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(result);
    }

    /// <summary>Connexion – retourne un JWT</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    /// <summary>Réinitialisation du mot de passe</summary>
    [HttpPost("reset-password")]
    [Authorize]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
    {
        var userId = GetCurrentUserId();
        await _authService.ResetPasswordAsync(userId, dto);
        return Ok(new { message = "Mot de passe modifié avec succès." });
    }

    // ─── Helper ──────────────────────────────────────────────────────────────
    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(claim, out var id))
            throw new UnauthorizedAccessException();
        return id;
    }
}