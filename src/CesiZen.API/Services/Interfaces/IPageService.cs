using CesiZen.API.DTOs.Pages;

namespace CesiZen.API.Services.Interfaces
{
    public interface IPageService
    {
        Task<IEnumerable<PageInfoDto>> GetAllAsync();
        Task<PageInfoDto?> GetBySlugAsync(string slug);
        Task<PageInfoDto> CreateAsync(CreateUpdatePageDto dto, int utilisateurId);
        Task<PageInfoDto> UpdateAsync(int id, CreateUpdatePageDto dto, int utilisateurId);
        Task DeleteAsync(int id);
    }
}