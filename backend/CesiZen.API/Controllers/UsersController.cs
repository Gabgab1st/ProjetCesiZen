using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CesiZen.API.DTOs.Users;
using CesiZen.API.Services.Interfaces;

namespace CesiZen.API.Controllers;

[ApiController] [Route("api/[controller]")] [Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) { _userService = userService; }
    private int GetCurrentUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

    [HttpGet("me")]
    public async Task<IActionResult> GetMe() { var u = await _userService.GetByIdAsync(GetCurrentUserId()); if (u == null) return NotFound(); return Ok(u); }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDto dto) { return Ok(await _userService.UpdateAsync(GetCurrentUserId(), dto)); }

    [HttpGet] [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> GetAll() { return Ok(await _userService.GetAllAsync()); }

    [HttpPost] [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Create([FromBody] AdminCreateUserDto dto) { var u = await _userService.AdminCreateAsync(dto); return CreatedAtAction(nameof(GetById), new { id = u.UtilisateurId }, u); }

    [HttpGet("{id:int}")] [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> GetById(int id) { var u = await _userService.GetByIdAsync(id); if (u == null) return NotFound(); return Ok(u); }

    [HttpPatch("{id:int}/active")] [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> SetActive(int id, [FromBody] bool actif) { await _userService.SetActiveAsync(id, actif); return NoContent(); }

    [HttpDelete("{id:int}")] [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Delete(int id) { await _userService.DeleteAsync(id); return NoContent(); }
}
