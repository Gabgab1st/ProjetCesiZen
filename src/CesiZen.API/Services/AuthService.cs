using CesiZen.API.Data;
using CesiZen.API.DTOs.Auth;
using CesiZen.API.Models;
using CesiZen.API.Services.Interfaces;
using CesiZen.API.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtSettings = CesiZen.API.Configuration.JwtSettings;

namespace CesiZen.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthService(AppDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var utilisateur = await _context.Utilisateurs
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (utilisateur == null || !utilisateur.Actif)
                throw new UnauthorizedAccessException("Identifiants invalides.");

            if (!BCrypt.Net.BCrypt.Verify(dto.MotDePasse, utilisateur.MotDePasseHashed))
                throw new UnauthorizedAccessException("Identifiants invalides.");

            var token = GenerateJwtToken(utilisateur);

            return new LoginResponseDto
            {
                Token = token,
                Email = utilisateur.Email,
                NomComplet = $"{utilisateur.Prenom} {utilisateur.Nom}",
                Role = utilisateur.Role.Nom
            };
        }

        public async Task RegisterAsync(RegisterRequestDto dto)
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
                RoleId = 2, // Rôle Utilisateur par défaut
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (utilisateur == null)
                throw new KeyNotFoundException("Aucun compte trouvé avec cet email.");

            utilisateur.MotDePasseHashed = BCrypt.Net.BCrypt.HashPassword(dto.NouveauMotDePasse);
            utilisateur.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private string GenerateJwtToken(Utilisateur utilisateur)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, utilisateur.UtilisateurId.ToString()),
                new Claim(ClaimTypes.Email, utilisateur.Email),
                new Claim(ClaimTypes.Role, utilisateur.Role.Nom),
                new Claim(ClaimTypes.Name, $"{utilisateur.Prenom} {utilisateur.Nom}")
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}