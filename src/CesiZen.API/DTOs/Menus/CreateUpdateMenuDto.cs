namespace CesiZen.API.DTOs.Menus
{
    public class CreateUpdateMenuDto
    {
        public string Libelle { get; set; } = string.Empty;
        public int Ordre { get; set; }
    }
}