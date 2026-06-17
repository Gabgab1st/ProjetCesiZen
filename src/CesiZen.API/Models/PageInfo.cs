namespace CesiZen.API.Models
{
    public class PageInfo
    {
        public int PageId { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Contenu { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = true;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Clés étrangères
        public int MenuId { get; set; }
        public int UtilisateurId { get; set; }

        // Navigation
        public Menu Menu { get; set; } = null!;
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}