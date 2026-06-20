using CesiZen.API.Data;
using CesiZen.API.DTOs.Pages;
using CesiZen.API.Models;
using CesiZen.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.API.Services
{
    public class PageService : IPageService
    {
        private readonly AppDbContext _context;

        public PageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PageInfoDto>> GetAllAsync()
        {
            return await _context.PagesInfo
                .Include(p => p.Menu)
                .Select(p => ToDto(p))
                .ToListAsync();
        }

        public async Task<PageInfoDto?> GetBySlugAsync(string slug)
        {
            var page = await _context.PagesInfo
                .Include(p => p.Menu)
                .FirstOrDefaultAsync(p => p.Slug == slug);

            return page == null ? null : ToDto(page);
        }

        public async Task<PageInfoDto> CreateAsync(CreateUpdatePageDto dto, int utilisateurId)
        {
            var page = new PageInfo
            {
                Titre = dto.Titre,
                Contenu = dto.Contenu,
                Slug = dto.Slug,
                IsPublic = dto.IsPublic,
                MenuId = dto.MenuId,
                UtilisateurId = utilisateurId,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PagesInfo.Add(page);
            await _context.SaveChangesAsync();
            await _context.Entry(page).Reference(p => p.Menu).LoadAsync();

            return ToDto(page);
        }

        public async Task<PageInfoDto> UpdateAsync(int id, CreateUpdatePageDto dto, int utilisateurId)
        {
            var page = await _context.PagesInfo
                .Include(p => p.Menu)
                .FirstOrDefaultAsync(p => p.PageId == id)
                ?? throw new KeyNotFoundException("Page introuvable.");

            page.Titre = dto.Titre;
            page.Contenu = dto.Contenu;
            page.Slug = dto.Slug;
            page.IsPublic = dto.IsPublic;
            page.MenuId = dto.MenuId;
            page.UtilisateurId = utilisateurId;
            page.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _context.Entry(page).Reference(p => p.Menu).LoadAsync();

            return ToDto(page);
        }

        public async Task DeleteAsync(int id)
        {
            var page = await _context.PagesInfo
                .FirstOrDefaultAsync(p => p.PageId == id)
                ?? throw new KeyNotFoundException("Page introuvable.");

            _context.PagesInfo.Remove(page);
            await _context.SaveChangesAsync();
        }

        private static PageInfoDto ToDto(PageInfo p) => new PageInfoDto
        {
            PageId = p.PageId,
            Titre = p.Titre,
            Contenu = p.Contenu,
            Slug = p.Slug,
            IsPublic = p.IsPublic,
            MenuId = p.MenuId, 
            MenuLibelle = p.Menu?.Libelle ?? string.Empty,
            UpdatedAt = p.UpdatedAt
        };
    }
}