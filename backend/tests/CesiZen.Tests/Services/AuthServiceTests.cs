using CesiZen.API.Configuration;
using CesiZen.API.Data;
using CesiZen.API.DTOs.Auth;
using CesiZen.API.Models;
using CesiZen.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace CesiZen.Tests
{
    public class AuthServiceTests
    {
        private AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new AppDbContext(options);
        }

        private IOptions<JwtSettings> GetJwtSettings() =>
            Options.Create(new JwtSettings
            {
                SecretKey = "CesiZenSuperSecretKeyForTesting123!",
                Issuer = "CesiZenAPI",
                Audience = "CesiZenApp",
                ExpirationHours = 24
            });

        private async Task SeedRoleAndAdmin(AppDbContext context)
        {
            context.Roles.AddRange(
                new Role { RoleId = 1, Nom = "Administrateur" },
                new Role { RoleId = 2, Nom = "Utilisateur" }
            );
            context.Utilisateurs.Add(new Utilisateur
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
            });
            await context.SaveChangesAsync();
        }

        // ── Login ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task Login_AvecCredentialsValides_RetourneToken()
        {
            using var context = CreateContext("Login_Valide");
            await SeedRoleAndAdmin(context);
            var service = new AuthService(context, GetJwtSettings());

            var result = await service.LoginAsync(new LoginRequestDto
            {
                Email = "admin@cesizen.fr",
                MotDePasse = "Admin1234!"
            });

            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal("admin@cesizen.fr", result.Email);
            Assert.Equal("Administrateur", result.Role);
        }

        [Fact]
        public async Task Login_AvecEmailInexistant_LeveUnauthorized()
        {
            using var context = CreateContext("Login_EmailInexistant");
            await SeedRoleAndAdmin(context);
            var service = new AuthService(context, GetJwtSettings());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.LoginAsync(new LoginRequestDto
                {
                    Email = "inconnu@cesizen.fr",
                    MotDePasse = "Admin1234!"
                }));
        }

        [Fact]
        public async Task Login_AvecMauvaisMotDePasse_LeveUnauthorized()
        {
            using var context = CreateContext("Login_MauvaisMdp");
            await SeedRoleAndAdmin(context);
            var service = new AuthService(context, GetJwtSettings());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.LoginAsync(new LoginRequestDto
                {
                    Email = "admin@cesizen.fr",
                    MotDePasse = "MauvaisMotDePasse!"
                }));
        }

        [Fact]
        public async Task Login_AvecCompteDesactive_LeveUnauthorized()
        {
            using var context = CreateContext("Login_CompteDesactive");
            context.Roles.Add(new Role { RoleId = 2, Nom = "Utilisateur" });
            context.Utilisateurs.Add(new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Test",
                Prenom = "User",
                Email = "user@cesizen.fr",
                MotDePasseHashed = BCrypt.Net.BCrypt.HashPassword("Test1234!"),
                Actif = false,
                RoleId = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            var service = new AuthService(context, GetJwtSettings());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.LoginAsync(new LoginRequestDto
                {
                    Email = "user@cesizen.fr",
                    MotDePasse = "Test1234!"
                }));
        }

        // ── Register ──────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_AvecNouvelEmail_CreeLUtilisateur()
        {
            using var context = CreateContext("Register_NouvelEmail");
            context.Roles.Add(new Role { RoleId = 2, Nom = "Utilisateur" });
            await context.SaveChangesAsync();
            var service = new AuthService(context, GetJwtSettings());

            await service.RegisterAsync(new RegisterRequestDto
            {
                Nom = "Vincent",
                Prenom = "Gabriel",
                Email = "gabriel@cesizen.fr",
                MotDePasse = "Test1234!"
            });

            var user = await context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == "gabriel@cesizen.fr");
            Assert.NotNull(user);
            Assert.Equal("Vincent", user.Nom);
            Assert.True(user.Actif);
            Assert.Equal(2, user.RoleId);
        }

        [Fact]
        public async Task Register_AvecEmailExistant_LeveInvalidOperation()
        {
            using var context = CreateContext("Register_EmailExistant");
            await SeedRoleAndAdmin(context);
            var service = new AuthService(context, GetJwtSettings());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.RegisterAsync(new RegisterRequestDto
                {
                    Nom = "Admin",
                    Prenom = "CesiZen",
                    Email = "admin@cesizen.fr",
                    MotDePasse = "Test1234!"
                }));
        }

        // ── ResetPassword ─────────────────────────────────────────────────────

        [Fact]
        public async Task ResetPassword_AvecEmailValide_ModifieLeMotDePasse()
        {
            using var context = CreateContext("ResetPassword_Valide");
            await SeedRoleAndAdmin(context);
            var service = new AuthService(context, GetJwtSettings());

            await service.ResetPasswordAsync(new ResetPasswordDto
            {
                Email = "admin@cesizen.fr",
                NouveauMotDePasse = "NouveauMdp1234!"
            });

            var user = await context.Utilisateurs.FirstAsync(u => u.Email == "admin@cesizen.fr");
            Assert.True(BCrypt.Net.BCrypt.Verify("NouveauMdp1234!", user.MotDePasseHashed));
        }

        [Fact]
        public async Task ResetPassword_AvecEmailInexistant_LeveKeyNotFound()
        {
            using var context = CreateContext("ResetPassword_EmailInexistant");
            await SeedRoleAndAdmin(context);
            var service = new AuthService(context, GetJwtSettings());

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.ResetPasswordAsync(new ResetPasswordDto
                {
                    Email = "inconnu@cesizen.fr",
                    NouveauMotDePasse = "NouveauMdp1234!"
                }));
        }
    }
}