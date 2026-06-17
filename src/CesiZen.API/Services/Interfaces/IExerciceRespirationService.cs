using CesiZen.API.DTOs.Exercices;

namespace CesiZen.API.Services.Interfaces
{
    public interface IExerciceRespirationService
    {
        Task<IEnumerable<ExerciceRespirationDto>> GetAllAsync();
        Task<ExerciceRespirationDto?> GetByIdAsync(int id);
        Task<ExerciceRespirationDto> CreateAsync(CreateUpdateExerciceDto dto, int utilisateurId);
        Task<ExerciceRespirationDto> UpdateAsync(int id, CreateUpdateExerciceDto dto);
        Task SetActiveAsync(int id, bool actif);
        Task DeleteAsync(int id);
    }
}