namespace CesiZen.API.Models
{
    public class Utilisateur
    {
        public int UtilisateurId { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MotDePasseHashed { get; set; } = string.Empty;
        public bool Actif { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Clé étrangère
        public int RoleId { get; set; }

        // Navigation
        public Role Role { get; set; } = null!;
        public ICollection<TokenReinitialisation> Tokens { get; set; } = new List<TokenReinitialisation>();
        public ICollection<PageInfo> Pages { get; set; } = new List<PageInfo>();
        public ICollection<ExerciceRespiration> Exercices { get; set; } = new List<ExerciceRespiration>();
    }
}