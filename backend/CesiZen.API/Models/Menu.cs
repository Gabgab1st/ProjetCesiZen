namespace CesiZen.API.Models
{
    public class Menu
    {
        public int MenuId { get; set; }
        public int Ordre { get; set; }
        public string Libelle { get; set; } = string.Empty;

        // Navigation
        public ICollection<PageInfo> Pages { get; set; } = new List<PageInfo>();
    }
}