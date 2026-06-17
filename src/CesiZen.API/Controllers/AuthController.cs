using CesiZen.API.DTOs.Auth;
using CesiZen.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) { _authService = authService; }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    { await _authService.RegisterAsync(dto); return Ok(new { message = "Compte cree." }); }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    { var result = await _authService.LoginAsync(dto); return Ok(result); }

    [HttpPost("reset-password")]
    [Authorize]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    { await _authService.ResetPasswordAsync(dto); return Ok(new { message = "MDP modifie." }); }
}