namespace CesiZen.API.DTOs.Exercices
{
    public class CreateUpdateExerciceDto
    {
        public string Nom { get; set; } = string.Empty;
        public int DureeInspiration { get; set; }
        public int DureeApnee { get; set; }
        public int DureeExpiration { get; set; }
    }
}