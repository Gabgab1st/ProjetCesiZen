using CesiZen.API.Data;
using CesiZen.API.DTOs.Menus;
using CesiZen.API.Models;
using CesiZen.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.API.Services
{
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;

        public MenuService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuDto>> GetAllAsync()
        {
            return await _context.Menus
                .OrderBy(m => m.Ordre)
                .Select(m => ToDto(m))
                .ToListAsync();
        }

        public async Task<MenuDto?> GetByIdAsync(int id)
        {
            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.MenuId == id);

            return menu == null ? null : ToDto(menu);
        }

        public async Task<MenuDto> CreateAsync(CreateUpdateMenuDto dto)
        {
            var menu = new Menu
            {
                Libelle = dto.Libelle,
                Ordre = dto.Ordre
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return ToDto(menu);
        }

        public async Task<MenuDto> UpdateAsync(int id, CreateUpdateMenuDto dto)
        {
            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.MenuId == id)
                ?? throw new KeyNotFoundException("Menu introuvable.");

            menu.Libelle = dto.Libelle;
            menu.Ordre = dto.Ordre;

            await _context.SaveChangesAsync();
            return ToDto(menu);
        }

        public async Task DeleteAsync(int id)
        {
            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.MenuId == id)
                ?? throw new KeyNotFoundException("Menu introuvable.");

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
        }

        private static MenuDto ToDto(Menu m) => new MenuDto
        {
            MenuId = m.MenuId,
            Libelle = m.Libelle,
            Ordre = m.Ordre
        };
    }
}