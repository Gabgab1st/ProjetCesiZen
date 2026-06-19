namespace CesiZen.API.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Nom { get; set; } = string.Empty;

        // Navigation
        public ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
    }
}