using CesiZen.API.Data;
using CesiZen.API.DTOs.Users;
using CesiZen.API.Models;
using CesiZen.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.API.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _context.Utilisateurs
                .Include(u => u.Role)
                .Select(u => ToDto(u))
                .ToListAsync();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var u = await _context.Utilisateurs
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UtilisateurId == id);

            return u == null ? null : ToDto(u);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
        {
            var utilisateur = await _context.Utilisateurs
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UtilisateurId == id)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");

            utilisateur.Nom = dto.Nom;
            utilisateur.Prenom = dto.Prenom;
            utilisateur.Email = dto.Email;
            utilisateur.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ToDto(utilisateur);
        }

        public async Task<UserDto> AdminCreateAsync(AdminCreateUserDto dto)
        {
            var exists = await _context.Utilisateurs
                .AnyAsync(u => u.Email == dto.Email);

            if (exists)
                throw new InvalidOperationException("Un compte existe déjà avec cet email.");

            var utilisateur = new Utilisateur
            {
                Nom = dto.Nom,
                Prenom = dto.Prenom,
                Email = dto.Email,
                MotDePasseHashed = BCrypt.Net.BCrypt.HashPassword(dto.MotDePasse),
                Actif = true,
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            await _context.Entry(utilisateur).Reference(u => u.Role).LoadAsync();
            return ToDto(utilisateur);
        }

        public async Task SetActiveAsync(int id, bool actif)
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.UtilisateurId == id)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");

            utilisateur.Actif = actif;
            utilisateur.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.UtilisateurId == id)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");

            _context.Utilisateurs.Remove(utilisateur);
            await _context.SaveChangesAsync();
        }

        private static UserDto ToDto(Utilisateur u) => new UserDto
        {
            UtilisateurId = u.UtilisateurId,
            Nom = u.Nom,
            Prenom = u.Prenom,
            Email = u.Email,
            Actif = u.Actif,
            Role = u.Role?.Nom ?? string.Empty,
            CreatedAt = u.CreatedAt
        };
    }
}