using CesiZen.API.Data;
using CesiZen.API.DTOs.Users;
using CesiZen.API.Models;
using CesiZen.API.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using Xunit;

namespace CesiZen.Tests
{
    public class UserServiceTests
    {
        private AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new AppDbContext(options);
        }

        private async Task SeedUsers(AppDbContext context)
        {
            context.Roles.AddRange(
                new Role { RoleId = 1, Nom = "Administrateur" },
                new Role { RoleId = 2, Nom = "Utilisateur" }
            );
            context.Utilisateurs.AddRange(
                new Utilisateur
                {
                    UtilisateurId = 1,
                    Nom = "Admin",
                    Prenom = "CesiZen",
                    Email = "admin@cesizen.fr",
                    MotDePasseHashed = BCrypt.Net.BCrypt.HashPassword("Admin1234!"),
                    Actif = true,
                    RoleId = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Utilisateur
                {
                    UtilisateurId = 2,
                    Nom = "Vincent",
                    Prenom = "Gabriel",
                    Email = "gabriel@cesizen.fr",
                    MotDePasseHashed = BCrypt.Net.BCrypt.HashPassword("Test1234!"),
                    Actif = true,
                    RoleId = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();
        }

        // ── GetAll ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_RetourneTousLesUtilisateurs()
        {
            using var context = CreateContext("User_GetAll");
            await SeedUsers(context);
            var service = new UserService(context);

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        // ── GetById ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_AvecIdValide_RetourneLUtilisateur()
        {
            using var context = CreateContext("User_GetById_Valide");
            await SeedUsers(context);
            var service = new UserService(context);

            var result = await service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("admin@cesizen.fr", result.Email);
        }

        [Fact]
        public async Task GetById_AvecIdInexistant_RetourneNull()
        {
            using var context = CreateContext("User_GetById_Inexistant");
            await SeedUsers(context);
            var service = new UserService(context);

            var result = await service.GetByIdAsync(999);

            Assert.Null(result);
        }

        // ── Update ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_AvecIdValide_ModifieLUtilisateur()
        {
            using var context = CreateContext("User_Update");
            await SeedUsers(context);
            var service = new UserService(context);

            var result = await service.UpdateAsync(2, new UpdateUserDto
            {
                Nom = "Vincent-Modifie",
                Prenom = "Gabriel",
                Email = "gabriel-modifie@cesizen.fr"
            });

            Assert.Equal("Vincent-Modifie", result.Nom);
            Assert.Equal("gabriel-modifie@cesizen.fr", result.Email);
        }

        [Fact]
        public async Task Update_AvecIdInexistant_LeveKeyNotFound()
        {
            using var context = CreateContext("User_Update_Inexistant");
            await SeedUsers(context);
            var service = new UserService(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.UpdateAsync(999, new UpdateUserDto
                {
                    Nom = "Test",
                    Prenom = "Test",
                    Email = "test@cesizen.fr"
                }));
        }

        // ── AdminCreate ───────────────────────────────────────────────────────

        [Fact]
        public async Task AdminCreate_AvecDtoValide_CreeLUtilisateur()
        {
            using var context = CreateContext("User_AdminCreate");
            await SeedUsers(context);
            var service = new UserService(context);

            var result = await service.AdminCreateAsync(new AdminCreateUserDto
            {
                Nom = "Nouveau",
                Prenom = "User",
                Email = "nouveau@cesizen.fr",
                MotDePasse = "Nouveau1234!",
                RoleId = 2
            });

            Assert.NotNull(result);
            Assert.Equal("nouveau@cesizen.fr", result.Email);
            Assert.Equal("Utilisateur", result.Role);
        }

        // ── SetActive ─────────────────────────────────────────────────────────

        [Fact]
        public async Task SetActive_DesactiveLUtilisateur()
        {
            using var context = CreateContext("User_SetActive_False");
            await SeedUsers(context);
            var service = new UserService(context);

            await service.SetActiveAsync(2, false);

            var user = await context.Utilisateurs.FindAsync(2);
            Assert.False(user!.Actif);
        }

        [Fact]
        public async Task SetActive_ActiveLUtilisateur()
        {
            using var context = CreateContext("User_SetActive_True");
            await SeedUsers(context);
            // Désactiver d'abord
            var user = await context.Utilisateurs.FindAsync(2);
            user!.Actif = false;
            await context.SaveChangesAsync();

            var service = new UserService(context);
            await service.SetActiveAsync(2, true);

            var updated = await context.Utilisateurs.FindAsync(2);
            Assert.True(updated!.Actif);
        }

        // ── Delete ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_AvecIdValide_SupprimeLUtilisateur()
        {
            using var context = CreateContext("User_Delete");
            await SeedUsers(context);
            var service = new UserService(context);

            await service.DeleteAsync(2);

            var user = await context.Utilisateurs.FindAsync(2);
            Assert.Null(user);
        }

        [Fact]
        public async Task Delete_AvecIdInexistant_LeveKeyNotFound()
        {
            using var context = CreateContext("User_Delete_Inexistant");
            await SeedUsers(context);
            var service = new UserService(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.DeleteAsync(999));
        }
    }
}