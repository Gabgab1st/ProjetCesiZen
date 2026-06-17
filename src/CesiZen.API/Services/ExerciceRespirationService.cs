using CesiZen.API.Data;
using CesiZen.API.DTOs.Exercices;
using CesiZen.API.Models;
using CesiZen.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.API.Services
{
    public class ExerciceRespirationService : IExerciceRespirationService
    {
        private readonly AppDbContext _context;

        public ExerciceRespirationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExerciceRespirationDto>> GetAllAsync()
        {
            return await _context.ExercicesRespiration
                .Where(e => e.Actif)
                .Select(e => ToDto(e))
                .ToListAsync();
        }

        public async Task<ExerciceRespirationDto?> GetByIdAsync(int id)
        {
            var exercice = await _context.ExercicesRespiration
                .FirstOrDefaultAsync(e => e.ExerciceId == id);

            return exercice == null ? null : ToDto(exercice);
        }

        public async Task<ExerciceRespirationDto> CreateAsync(CreateUpdateExerciceDto dto, int utilisateurId)
        {
            var exercice = new ExerciceRespiration
            {
                Nom = dto.Nom,
                DureeInspiration = dto.DureeInspiration,
                DureeApnee = dto.DureeApnee,
                DureeExpiration = dto.DureeExpiration,
                Actif = true,
                UtilisateurId = utilisateurId
            };

            _context.ExercicesRespiration.Add(exercice);
            await _context.SaveChangesAsync();
            return ToDto(exercice);
        }

        public async Task<ExerciceRespirationDto> UpdateAsync(int id, CreateUpdateExerciceDto dto)
        {
            var exercice = await _context.ExercicesRespiration
                .FirstOrDefaultAsync(e => e.ExerciceId == id)
                ?? throw new KeyNotFoundException("Exercice introuvable.");

            exercice.Nom = dto.Nom;
            exercice.DureeInspiration = dto.DureeInspiration;
            exercice.DureeApnee = dto.DureeApnee;
            exercice.DureeExpiration = dto.DureeExpiration;

            await _context.SaveChangesAsync();
            return ToDto(exercice);
        }

        public async Task SetActiveAsync(int id, bool actif)
        {
            var exercice = await _context.ExercicesRespiration
                .FirstOrDefaultAsync(e => e.ExerciceId == id)
                ?? throw new KeyNotFoundException("Exercice introuvable.");

            exercice.Actif = actif;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var exercice = await _context.ExercicesRespiration
                .FirstOrDefaultAsync(e => e.ExerciceId == id)
                ?? throw new KeyNotFoundException("Exercice introuvable.");

            _context.ExercicesRespiration.Remove(exercice);
            await _context.SaveChangesAsync();
        }

        private static ExerciceRespirationDto ToDto(ExerciceRespiration e) => new ExerciceRespirationDto
        {
            ExerciceId = e.ExerciceId,
            Nom = e.Nom,
            DureeInspiration = e.DureeInspiration,
            DureeApnee = e.DureeApnee,
            DureeExpiration = e.DureeExpiration,
            Actif = e.Actif
        };
    }
}