using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CesiZen.API.DTOs.Exercices;
using CesiZen.API.Services.Interfaces;

namespace CesiZen.API.Controllers;

[ApiController] [Route("api/exercices-respiration")]
public class ExercicesRespirationController : ControllerBase
{
    private readonly IExerciceRespirationService _exerciceService;
    public ExercicesRespirationController(IExerciceRespirationService exerciceService) { _exerciceService = exerciceService; }
    private int GetCurrentUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

    [HttpGet] [AllowAnonymous]
    public async Task<IActionResult> GetAll() { return Ok(await _exerciceService.GetAllAsync()); }

    [HttpGet("{id:int}")] [AllowAnonymous]
    public async Task<IActionResult> GetById(int id) { var e = await _exerciceService.GetByIdAsync(id); if (e == null) return NotFound(); return Ok(e); }

    [HttpPost] [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Create([FromBody] CreateUpdateExerciceDto dto) { var e = await _exerciceService.CreateAsync(dto, GetCurrentUserId()); return CreatedAtAction(nameof(GetById), new { id = e.ExerciceId }, e); }

    [HttpPut("{id:int}")] [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateExerciceDto dto) { return Ok(await _exerciceService.UpdateAsync(id, dto)); }

    [HttpPatch("{id:int}/active")] [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> SetActive(int id, [FromBody] bool actif) { await _exerciceService.SetActiveAsync(id, actif); return NoContent(); }

    [HttpDelete("{id:int}")] [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Delete(int id) { await _exerciceService.DeleteAsync(id); return NoContent(); }
}
