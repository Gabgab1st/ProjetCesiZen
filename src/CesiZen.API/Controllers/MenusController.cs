using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CesiZen.API.DTOs.Menus;
using CesiZen.API.Services.Interfaces;

namespace CesiZen.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>Liste tous les menus (public)</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var menus = await _menuService.GetAllAsync();
        return Ok(menus);
    }

    /// <summary>Récupère un menu par son id (public)</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var menu = await _menuService.GetByIdAsync(id);
        return Ok(menu);
    }

    /// <summary>[Admin] Crée un menu</summary>
    [HttpPost]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Create([FromBody] CreateUpdateMenuDto dto)
    {
        var menu = await _menuService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = menu.MenuId }, menu);
    }

    /// <summary>[Admin] Met à jour un menu</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateMenuDto dto)
    {
        var menu = await _menuService.UpdateAsync(id, dto);
        return Ok(menu);
    }

    /// <summary>[Admin] Supprime un menu</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrateur")]
    public async Task<IActionResult> Delete(int id)
    {
        await _menuService.DeleteAsync(id);
        return NoContent();
    }
}