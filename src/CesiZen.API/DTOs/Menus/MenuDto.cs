namespace CesiZen.API.DTOs.Menus
{
    public class MenuDto
    {
        public int MenuId { get; set; }
        public string Libelle { get; set; } = string.Empty;
        public int Ordre { get; set; }
    }
}