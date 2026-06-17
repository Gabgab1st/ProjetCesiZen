using CesiZen.API.Data;
using CesiZen.API.DTOs.Exercices;
using CesiZen.API.Models;
using CesiZen.API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CesiZen.Tests.Services;

/// <summary>
/// Tests unitaires du service des exercices de respiration.
/// </summary>
public class ExerciceRespirationServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ExerciceRespirationService _service;

    public ExerciceRespirationServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        // Seed : 1 admin + 3 exercices (dont 1 inactif)
        _context.Roles.Add(new Role { RoleId = 1, Nom = "Administrateur" });
        _context.Utilisateurs.Add(new Utilisateur
        {
            UtilisateurId = 1,
            Nom = "Admin",
            Prenom = "Super",
            Email = "admin@cesizen.fr",
            MotDePasseHashed = BCrypt.Net.BCrypt.HashPassword("Admin1234!"),
            Actif = true,
            RoleId = 1
        });
        _context.ExercicesRespiration.AddRange(
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
                Nom = "Désactivé",
                DureeInspiration = 3,
                DureeApnee = 0,
                DureeExpiration = 3,
                Actif = false, // inactif
                UtilisateurId = 1
            }
        );
        _context.SaveChanges();

        _service = new ExerciceRespirationService(_context);
    }

    // ─── GET ALL (actifs uniquement) ──────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_RetourneSeulementLesExercicesActifs()
    {
        var result = (await _service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.True(e.Actif));
    }

    // ─── GET BY ID ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_IdExistant_RetourneLExercice()
    {
        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("748", result.Nom);
        Assert.Equal(7, result.DureeInspiration);
        Assert.Equal(4, result.DureeApnee);
        Assert.Equal(8, result.DureeExpiration);
    }

    [Fact]
    public async Task GetByIdAsync_IdInexistant_RetourneNull()
    {
        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    // ─── CREATE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_DonneesValides_CreeLExercice()
    {
        var dto = new CreateUpdateExerciceDto
        {
            Nom = "46",
            DureeInspiration = 4,
            DureeApnee = 0,
            DureeExpiration = 6
        };

        var result = await _service.CreateAsync(dto, utilisateurId: 1);

        Assert.NotNull(result);
        Assert.Equal("46", result.Nom);
        Assert.Equal(4, result.DureeInspiration);
        Assert.Equal(6, result.DureeExpiration);
        Assert.True(result.Actif); // Actif par défaut à la création

        // Vérifier persistance en base
        var enBase = await _context.ExercicesRespiration
            .FirstOrDefaultAsync(e => e.Nom == "46");
        Assert.NotNull(enBase);
    }

    // ─── UPDATE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_DonneesValides_MetAJourLExercice()
    {
        var dto = new CreateUpdateExerciceDto
        {
            Nom = "748 modifié",
            DureeInspiration = 7,
            DureeApnee = 4,
            DureeExpiration = 9 // changement
        };

        var result = await _service.UpdateAsync(1, dto);

        Assert.Equal("748 modifié", result.Nom);
        Assert.Equal(9, result.DureeExpiration);

        var enBase = await _context.ExercicesRespiration.FindAsync(1);
        Assert.Equal(9, enBase!.DureeExpiration);
    }

    [Fact]
    public async Task UpdateAsync_IdInexistant_LeveKeyNotFound()
    {
        var dto = new CreateUpdateExerciceDto { Nom = "Test" };

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateAsync(999, dto));
    }

    // ─── SET ACTIVE ───────────────────────────────────────────────────────────

    [Fact]
    public async Task SetActiveAsync_DesactiveUnExerciceActif()
    {
        await _service.SetActiveAsync(1, false);

        var exercice = await _context.ExercicesRespiration.FindAsync(1);
        Assert.False(exercice!.Actif);
    }

    [Fact]
    public async Task SetActiveAsync_ReactiveUnExerciceInactif()
    {
        await _service.SetActiveAsync(3, true); // exercice inactif en seed

        var exercice = await _context.ExercicesRespiration.FindAsync(3);
        Assert.True(exercice!.Actif);
    }

    [Fact]
    public async Task SetActiveAsync_IdInexistant_LeveKeyNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.SetActiveAsync(999, false));
    }

    // ─── DELETE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ExerciceExistant_LeSupprimeDeLaBase()
    {
        await _service.DeleteAsync(1);

        var supprime = await _context.ExercicesRespiration.FindAsync(1);
        Assert.Null(supprime);
    }

    [Fact]
    public async Task DeleteAsync_IdInexistant_LeveKeyNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.DeleteAsync(999));
    }

    // ─── Cleanup ──────────────────────────────────────────────────────────────
    public void Dispose() => _context.Dispose();
}