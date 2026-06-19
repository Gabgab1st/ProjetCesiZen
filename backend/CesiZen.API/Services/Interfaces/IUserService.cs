using CesiZen.API.DTOs.Users;

namespace CesiZen.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto> UpdateAsync(int id, UpdateUserDto dto);
        Task<UserDto> AdminCreateAsync(AdminCreateUserDto dto);
        Task SetActiveAsync(int id, bool actif);
        Task DeleteAsync(int id);
    }
}