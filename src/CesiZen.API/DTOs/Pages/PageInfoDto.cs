namespace CesiZen.API.DTOs.Pages
{
    public class PageInfoDto
    {
        public int PageId { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Contenu { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public string MenuLibelle { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}