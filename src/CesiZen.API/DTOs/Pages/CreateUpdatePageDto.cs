namespace CesiZen.API.DTOs.Pages
{
    public class CreateUpdatePageDto
    {
        public string Titre { get; set; } = string.Empty;
        public string Contenu { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public int MenuId { get; set; }
    }
}