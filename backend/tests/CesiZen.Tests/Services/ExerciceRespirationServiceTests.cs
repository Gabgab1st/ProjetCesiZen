using CesiZen.API.Data;
using CesiZen.API.DTOs.Exercices;
using CesiZen.API.Models;
using CesiZen.API.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using Xunit;

namespace CesiZen.Tests
{
    public class ExerciceRespirationServiceTests
    {
        private AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new AppDbContext(options);
        }

        private async Task SeedExercices(AppDbContext context)
        {
            context.Roles.Add(new Role { RoleId = 1, Nom = "Administrateur" });
            context.Utilisateurs.Add(new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Admin",
                Prenom = "CesiZen",
                Email = "admin@cesizen.fr",
                MotDePasseHashed = "hashed",
                Actif = true,
                RoleId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            context.ExercicesRespiration.AddRange(
                new ExerciceRespiration
                {
                    ExerciceId = 1,
                    Nom = "748",
                    DureeInspiration = 7,
                    DureeApnee = 4,
                    DureeExpiration = 8,
                    Actif = true,
                    UtilisateurId = 1
                },
                new ExerciceRespiration
                {
                    ExerciceId = 2,
                    Nom = "55",
                    DureeInspiration = 5,
                    DureeApnee = 0,
                    DureeExpiration = 5,
                    Actif = true,
                    UtilisateurId = 1
                },
                new ExerciceRespiration
                {
                    ExerciceId = 3,
                    Nom = "Desactive",
                    DureeInspiration = 4,
                    DureeApnee = 0,
                    DureeExpiration = 6,
                    Actif = false,
                    UtilisateurId = 1
                }
            );
            await context.SaveChangesAsync();
        }

        // ── GetAll ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_RetourneSeulementLesExercicesActifs()
        {
            using var context = CreateContext("Exercice_GetAll");
            await SeedExercices(context);
            var service = new ExerciceRespirationService(context);

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.All(result, e => Assert.True(e.Actif));
        }

        // ── GetById ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_AvecIdValide_RetourneLExercice()
        {
            using var context = CreateContext("Exercice_GetById_Valide");
            await SeedExercices(context);
            var service = new ExerciceRespirationService(context);

            var result = await service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("748", result.Nom);
            Assert.Equal(7, result.DureeInspiration);
        }

        [Fact]
        public async Task GetById_AvecIdInexistant_RetourneNull()
        {
            using var context = CreateContext("Exercice_GetById_Inexistant");
            await SeedExercices(context);
            var service = new ExerciceRespirationService(context);

            var result = await service.GetByIdAsync(999);

            Assert.Null(result);
        }

        // ── Create ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_AvecDtoValide_CreeLExercice()
        {
            using var context = CreateContext("Exercice_Create");
            await SeedExercices(context);
            var service = new ExerciceRespirationService(context);

            var result = await service.CreateAsync(new CreateUpdateExerciceDto
            {
                Nom = "46",
                DureeInspiration = 4,
                DureeApnee = 0,
                DureeExpiration = 6
            }, utilisateurId: 1);

            Assert.NotNull(result);
            Assert.Equal("46", result.Nom);
            Assert.True(result.Actif);
        }

        // ── Update ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_AvecIdValide_ModifieLExercice()
        {
            using var context = CreateContext("Exercice_Update");
            await SeedExercices(context);
            var service = new ExerciceRespirationService(context);

            var result = await service.UpdateAsync(1, new CreateUpdateExerciceDto
            {
                Nom = "748-modifie",
                DureeInspiration = 8,
                DureeApnee = 4,
                DureeExpiration = 8
            });

            Assert.Equal("748-modifie", result.Nom);
            Assert.Equal(8, result.DureeInspiration);
        }

        [Fact]
        public async Task Update_AvecIdInexistant_LeveKeyNotFound()
        {
            using var context = CreateContext("Exercice_Update_Inexistant");
            await SeedExercices(context);
            var service = new ExerciceRespirationService(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.UpdateAsync(999, new CreateUpdateExerciceDto
                {
                    Nom = "inexistant",
                    DureeInspiration = 5,
                    DureeApnee = 0,
                    DureeExpiration = 5
                }));
        }

        // ── SetActive ─────────────────────────────────────────────────────────

        [Fact]
        public async Task SetActive_DesactiveLExercice()
        {
            using var context = CreateContext("Exercice_SetActive_False");
            await SeedExercices(context);
            var service = new ExerciceRespirationService(context);

            await service.SetActiveAsync(1, false);

            var exercice = await context.ExercicesRespiration.FindAsync(1);
            Assert.False(exercice!.Actif);
        }

        [Fact]
        public async Task SetActive_ActiveLExercice()
        {
            using var context = CreateContext("Exercice_SetActive_True");
            await SeedExercices(context);
            var service = new ExerciceRespirationService(context);

            await service.SetActiveAsync(3, true);

            var exercice = await context.ExercicesRespiration.FindAsync(3);
            Assert.True(exercice!.Actif);
        }

        // ── Delete ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_AvecIdValide_SupprimeLExercice()
        {
            using var context = CreateContext("Exercice_Delete");
            await SeedExercices(context);
            var service = new ExerciceRespirationService(context);

            await service.DeleteAsync(1);

            var exercice = await context.ExercicesRespiration.FindAsync(1);
            Assert.Null(exercice);
        }

        [Fact]
        public async Task Delete_AvecIdInexistant_LeveKeyNotFound()
        {
            using var context = CreateContext("Exercice_Delete_Inexistant");
            await SeedExercices(context);
            var service = new ExerciceRespirationService(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.DeleteAsync(999));
        }
    }
}