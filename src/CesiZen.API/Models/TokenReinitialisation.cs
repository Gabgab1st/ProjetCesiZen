namespace CesiZen.API.Models
{
    public class TokenReinitialisation
    {
        public int TokenId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireA { get; set; }

        // Clé étrangère
        public int UtilisateurId { get; set; }

        // Navigation
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}