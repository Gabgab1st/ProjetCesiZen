namespace CesiZen.API.Models
{
    public class ExerciceRespiration
    {
        public int ExerciceId { get; set; }
        public string Nom { get; set; } = string.Empty;
        public int DureeInspiration { get; set; }
        public int DureeApnee { get; set; }
        public int DureeExpiration { get; set; }
        public bool Actif { get; set; } = true;

        // Clé étrangère
        public int UtilisateurId { get; set; }

        // Navigation
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}