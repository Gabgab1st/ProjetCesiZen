using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CesiZen.API.DTOs.Users;
using CesiZen.API.Services.Interfaces;

namespace CesiZen.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Récupère le profil de l'utilisateur connecté</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var id = GetCurrentUserId();
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

    /// <summary>Met à jour le profil de l'utilisateur connecté</summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDto dto)
    {
        var id = GetCurrentUserId();
        var user = await _userService.UpdateAsync(id, dto);
        return Ok(user);
    }

    // ─── Admin ───────────────────────────────────────────────────────────────

    /// <summary>[Admin] Liste tous les utilisateurs</summary>
    [HttpGet]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    /// <summary>[Admin] Crée un utilisateur / administrateur</summary>
    [HttpPost]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Create([FromBody] AdminCreateUserDto dto)
    {
        var user = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = user.UtilisateurId }, user);
    }

    /// <summary>[Admin] Récupère un utilisateur par son id</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

    /// <summary>[Admin] Active / désactive un compte</summary>
    [HttpPatch("{id:int}/active")]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> SetActive(int id, [FromBody] bool actif)
    {
        await _userService.SetActiveAsync(id, actif);
        return NoContent();
    }

    /// <summary>[Admin] Supprime un compte</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Delete(int id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
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