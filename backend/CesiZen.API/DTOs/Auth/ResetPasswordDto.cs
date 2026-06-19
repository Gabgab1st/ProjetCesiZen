namespace CesiZen.API.DTOs.Auth
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string NouveauMotDePasse { get; set; } = string.Empty;
    }
}