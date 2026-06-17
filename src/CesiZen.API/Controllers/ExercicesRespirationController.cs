using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CesiZen.API.DTOs.Exercices;
using CesiZen.API.Services.Interfaces;

namespace CesiZen.API.Controllers;

[ApiController]
[Route("api/exercices-respiration")]
public class ExercicesRespirationController : ControllerBase
{
    private readonly IExerciceRespirationService _exerciceService;

    public ExercicesRespirationController(IExerciceRespirationService exerciceService)
    {
        _exerciceService = exerciceService;
    }

    /// <summary>Liste les exercices actifs (public)</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var exercices = await _exerciceService.GetAllAsync();
        return Ok(exercices);
    }

    /// <summary>Récupère un exercice par son id (public)</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var exercice = await _exerciceService.GetByIdAsync(id);
        if (exercice == null) return NotFound();
        return Ok(exercice);
    }

    /// <summary>[Admin] Crée un exercice</summary>
    [HttpPost]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Create([FromBody] CreateUpdateExerciceDto dto)
    {
        var userId = GetCurrentUserId();
        var exercice = await _exerciceService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = exercice.ExerciceId }, exercice);
    }

    /// <summary>[Admin] Met à jour un exercice</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateExerciceDto dto)
    {
        var exercice = await _exerciceService.UpdateAsync(id, dto);
        return Ok(exercice);
    }

    /// <summary>[Admin] Active / désactive un exercice</summary>
    [HttpPatch("{id:int}/active")]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> SetActive(int id, [FromBody] bool actif)
    {
        await _exerciceService.SetActiveAsync(id, actif);
        return NoContent();
    }

    /// <summary>[Admin] Supprime un exercice</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Delete(int id)
    {
        await _exerciceService.DeleteAsync(id);
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