using CesiZen.API.Data;
using CesiZen.API.DTOs.Users;
using CesiZen.API.Models;
using CesiZen.API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CesiZen.Tests.Services;

/// <summary>
/// Tests unitaires du service de gestion des utilisateurs.
/// </summary>
public class UserServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _context.Roles.AddRange(
            new Role { RoleId = 1, Nom = "Administrateur" },
            new Role { RoleId = 2, Nom = "Utilisateur" }
        );
        _context.Utilisateurs.AddRange(
            new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Dupont",
                Prenom = "Jean",
                Email = "jean@test.fr",
                MotDePasseHashed = BCrypt.Net.BCrypt.HashPassword("Pass1!"),
                Actif = true,
                RoleId = 2
            },
            new Utilisateur
            {
                UtilisateurId = 2,
                Nom = "Admin",
                Prenom = "Super",
                Email = "admin@cesizen.fr",
                MotDePasseHashed = BCrypt.Net.BCrypt.HashPassword("Admin1234!"),
                Actif = true,
                RoleId = 1
            }
        );
        _context.SaveChanges();

        _userService = new UserService(_context);
    }

    // ─── GET ALL ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_RetourneTousLesUtilisateurs()
    {
        var result = await _userService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    // ─── GET BY ID ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_IdExistant_RetourneUtilisateur()
    {
        var result = await _userService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Dupont", result.Nom);
        Assert.Equal("jean@test.fr", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_IdInexistant_RetourneNull()
    {
        var result = await _userService.GetByIdAsync(999);

        Assert.Null(result);
    }

    // ─── UPDATE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_DonneesValides_MetAJourLUtilisateur()
    {
        var dto = new UpdateUserDto
        {
            Nom = "Durand",
            Prenom = "Jean-Pierre",
            Email = "jp.durand@test.fr"
        };

        var result = await _userService.UpdateAsync(1, dto);

        Assert.Equal("Durand", result.Nom);
        Assert.Equal("Jean-Pierre", result.Prenom);
        Assert.Equal("jp.durand@test.fr", result.Email);

        // Vérifier la persistance en base
        var enBase = await _context.Utilisateurs.FindAsync(1);
        Assert.Equal("Durand", enBase!.Nom);
    }

    [Fact]
    public async Task UpdateAsync_IdInexistant_LeveKeyNotFound()
    {
        var dto = new UpdateUserDto { Nom = "X", Prenom = "Y", Email = "x@y.fr" };

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _userService.UpdateAsync(999, dto));
    }

    // ─── ADMIN CREATE ─────────────────────────────────────────────────────────

    [Fact]
    public async Task AdminCreateAsync_NouvelEmail_CreeLUtilisateur()
    {
        var dto = new AdminCreateUserDto
        {
            Nom = "Nouveau",
            Prenom = "Test",
            Email = "nouveau@test.fr",
            MotDePasse = "NewPass123!",
            RoleId = 2
        };

        var result = await _userService.AdminCreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Nouveau", result.Nom);
        Assert.Equal("Utilisateur", result.Role);
        Assert.True(result.Actif);
    }

    [Fact]
    public async Task AdminCreateAsync_EmailExistant_LeveInvalidOperation()
    {
        var dto = new AdminCreateUserDto
        {
            Nom = "Doublon",
            Prenom = "Test",
            Email = "jean@test.fr", // email déjà en base
            MotDePasse = "Pass!",
            RoleId = 2
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.AdminCreateAsync(dto));
    }

    // ─── SET ACTIVE ───────────────────────────────────────────────────────────

    [Fact]
    public async Task SetActiveAsync_DesactiveUnCompte()
    {
        await _userService.SetActiveAsync(1, false);

        var utilisateur = await _context.Utilisateurs.FindAsync(1);
        Assert.False(utilisateur!.Actif);
    }

    [Fact]
    public async Task SetActiveAsync_ReactiveUnCompte()
    {
        // D'abord désactiver
        var u = await _context.Utilisateurs.FindAsync(1);
        u!.Actif = false;
        await _context.SaveChangesAsync();

        await _userService.SetActiveAsync(1, true);

        var result = await _context.Utilisateurs.FindAsync(1);
        Assert.True(result!.Actif);
    }

    [Fact]
    public async Task SetActiveAsync_IdInexistant_LeveKeyNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _userService.SetActiveAsync(999, false));
    }

    // ─── DELETE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_UtilisateurExistant_LeSupprimeDeLaBase()
    {
        await _userService.DeleteAsync(1);

        var supprime = await _context.Utilisateurs.FindAsync(1);
        Assert.Null(supprime);
    }

    [Fact]
    public async Task DeleteAsync_IdInexistant_LeveKeyNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _userService.DeleteAsync(999));
    }

    // ─── Cleanup ──────────────────────────────────────────────────────────────
    public void Dispose() => _context.Dispose();
}