namespace CesiZen.API.DTOs.Users
{
    public class AdminCreateUserDto
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}