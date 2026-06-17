using CesiZen.API.Configuration;
using CesiZen.API.Data;
using CesiZen.API.DTOs.Auth;
using CesiZen.API.Models;
using CesiZen.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace CesiZen.Tests.Services;

/// <summary>
/// Tests unitaires du service d'authentification.
/// Utilise une base de données InMemory (pas de vraie connexion MySQL).
/// </summary>
public class AuthServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;

    // ─── Constantes de test ───────────────────────────────────────────────────
    private const string EmailExistant = "jean.dupont@test.fr";
    private const string MotDePasseClair = "Password123!";

    public AuthServiceTests()
    {
        // Base InMemory isolée par test (nom unique)
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        // Seed minimal : 2 rôles + 1 utilisateur actif
        _context.Roles.AddRange(
            new Role { RoleId = 1, Nom = "Administrateur" },
            new Role { RoleId = 2, Nom = "Utilisateur" }
        );
        _context.Utilisateurs.Add(new Utilisateur
        {
            UtilisateurId = 1,
            Nom = "Dupont",
            Prenom = "Jean",
            Email = EmailExistant,
            MotDePasseHashed = BCrypt.Net.BCrypt.HashPassword(MotDePasseClair),
            Actif = true,
            RoleId = 2
        });
        _context.SaveChanges();

        var jwtOptions = Options.Create(new JwtSettings
        {
            SecretKey = "CesiZen_TestSecretKey_2024_MinLength32!!",
            Issuer = "CesiZenAPI",
            Audience = "CesiZenApp",
            ExpirationHours = 1
        });

        _authService = new AuthService(_context, jwtOptions);
    }

    // ─── LOGIN ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_IdentifiantsValides_RetourneToken()
    {
        var dto = new LoginRequestDto
        {
            Email = EmailExistant,
            MotDePasse = MotDePasseClair
        };

        var result = await _authService.LoginAsync(dto);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Token));
        Assert.Equal(EmailExistant, result.Email);
        Assert.Equal("Utilisateur", result.Role);
    }

    [Fact]
    public async Task LoginAsync_EmailInexistant_LeveUnauthorized()
    {
        var dto = new LoginRequestDto
        {
            Email = "inconnu@test.fr",
            MotDePasse = MotDePasseClair
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_MauvaisMotDePasse_LeveUnauthorized()
    {
        var dto = new LoginRequestDto
        {
            Email = EmailExistant,
            MotDePasse = "MauvaisMotDePasse!"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_CompteInactif_LeveUnauthorized()
    {
        // Désactiver le compte
        var utilisateur = await _context.Utilisateurs.FindAsync(1);
        utilisateur!.Actif = false;
        await _context.SaveChangesAsync();

        var dto = new LoginRequestDto
        {
            Email = EmailExistant,
            MotDePasse = MotDePasseClair
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(dto));
    }

    // ─── REGISTER ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task RegisterAsync_NouvelUtilisateur_CreeLEntree()
    {
        var dto = new RegisterRequestDto
        {
            Nom = "Martin",
            Prenom = "Alice",
            Email = "alice.martin@test.fr",
            MotDePasse = "Secure456!"
        };

        await _authService.RegisterAsync(dto);

        var cree = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email == "alice.martin@test.fr");

        Assert.NotNull(cree);
        Assert.Equal("Martin", cree.Nom);
        Assert.Equal("Alice", cree.Prenom);
        Assert.True(cree.Actif);
        Assert.Equal(2, cree.RoleId); // Rôle Utilisateur par défaut
        // Le mot de passe doit être hashé (pas stocké en clair)
        Assert.NotEqual("Secure456!", cree.MotDePasseHashed);
        Assert.True(BCrypt.Net.BCrypt.Verify("Secure456!", cree.MotDePasseHashed));
    }

    [Fact]
    public async Task RegisterAsync_EmailDejaUtilise_LeveInvalidOperation()
    {
        var dto = new RegisterRequestDto
        {
            Nom = "Autre",
            Prenom = "Personne",
            Email = EmailExistant, // email déjà en base
            MotDePasse = "AutrePass123!"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.RegisterAsync(dto));
    }

    // ─── RESET PASSWORD ───────────────────────────────────────────────────────

    [Fact]
    public async Task ResetPasswordAsync_EmailValide_HasheLeMDP()
    {
        var dto = new ResetPasswordDto
        {
            Email = EmailExistant,
            NouveauMotDePasse = "NouveauPass789!"
        };

        await _authService.ResetPasswordAsync(dto);

        var utilisateur = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email == EmailExistant);

        Assert.NotNull(utilisateur);
        Assert.True(BCrypt.Net.BCrypt.Verify("NouveauPass789!", utilisateur.MotDePasseHashed));
    }

    [Fact]
    public async Task ResetPasswordAsync_EmailInconnu_LeveKeyNotFound()
    {
        var dto = new ResetPasswordDto
        {
            Email = "fantome@test.fr",
            NouveauMotDePasse = "NouveauPass789!"
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _authService.ResetPasswordAsync(dto));
    }

    // ─── Cleanup ──────────────────────────────────────────────────────────────
    public void Dispose() => _context.Dispose();
}