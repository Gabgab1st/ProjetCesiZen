namespace CesiZen.API.DTOs.Auth
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
    }
}