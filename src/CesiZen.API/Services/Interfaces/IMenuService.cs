using CesiZen.API.DTOs.Menus;

namespace CesiZen.API.Services.Interfaces
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuDto>> GetAllAsync();
        Task<MenuDto?> GetByIdAsync(int id);
        Task<MenuDto> CreateAsync(CreateUpdateMenuDto dto);
        Task<MenuDto> UpdateAsync(int id, CreateUpdateMenuDto dto);
        Task DeleteAsync(int id);
    }
}