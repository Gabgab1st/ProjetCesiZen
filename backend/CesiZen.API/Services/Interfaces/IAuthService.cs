using CesiZen.API.DTOs.Auth;

namespace CesiZen.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task RegisterAsync(RegisterRequestDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
    }
}