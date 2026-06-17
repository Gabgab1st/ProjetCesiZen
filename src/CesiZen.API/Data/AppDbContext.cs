using CesiZen.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<TokenReinitialisation> TokensReinitialisation { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<PageInfo> PagesInfo { get; set; }
        public DbSet<ExerciceRespiration> ExercicesRespiration { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Role ──────────────────────────────────────────────
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.RoleId);
                entity.Property(r => r.Nom)
                      .IsRequired()
                      .HasMaxLength(50);
            });

            // ── Utilisateur ───────────────────────────────────────
            modelBuilder.Entity<Utilisateur>(entity =>
            {
                entity.HasKey(u => u.UtilisateurId);

                entity.Property(u => u.Nom)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Prenom)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.MotDePasseHashed)
                      .IsRequired();

                entity.Property(u => u.Actif)
                      .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValue(DateTime.UtcNow);

                entity.Property(u => u.UpdatedAt)
                      .HasDefaultValue(DateTime.UtcNow);

                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Utilisateurs)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── TokenReinitialisation ─────────────────────────────
            modelBuilder.Entity<TokenReinitialisation>(entity =>
            {
                entity.HasKey(t => t.TokenId);

                entity.Property(t => t.Token)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(t => t.ExpireA)
                      .IsRequired();

                entity.HasOne(t => t.Utilisateur)
                      .WithMany(u => u.Tokens)
                      .HasForeignKey(t => t.UtilisateurId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Menu ──────────────────────────────────────────────
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(m => m.MenuId);

                entity.Property(m => m.Libelle)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(m => m.Ordre)
                      .IsRequired();
            });

            // ── PageInfo ──────────────────────────────────────────
            modelBuilder.Entity<PageInfo>(entity =>
            {
                entity.HasKey(p => p.PageId);

                entity.Property(p => p.Titre)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Contenu)
                      .IsRequired();

                entity.Property(p => p.Slug)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.HasIndex(p => p.Slug)
                      .IsUnique();

                entity.Property(p => p.IsPublic)
                      .HasDefaultValue(true);

                entity.Property(p => p.UpdatedAt)
                      .HasDefaultValue(DateTime.UtcNow);

                entity.HasOne(p => p.Menu)
                      .WithMany(m => m.Pages)
                      .HasForeignKey(p => p.MenuId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Utilisateur)
                      .WithMany(u => u.Pages)
                      .HasForeignKey(p => p.UtilisateurId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── ExerciceRespiration ───────────────────────────────
            modelBuilder.Entity<ExerciceRespiration>(entity =>
            {
                entity.HasKey(e => e.ExerciceId);

                entity.Property(e => e.Nom)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.DureeInspiration)
                      .IsRequired();

                entity.Property(e => e.DureeApnee)
                      .IsRequired();

                entity.Property(e => e.DureeExpiration)
                      .IsRequired();

                entity.Property(e => e.Actif)
                      .HasDefaultValue(true);

                entity.HasOne(e => e.Utilisateur)
                      .WithMany(u => u.Exercices)
                      .HasForeignKey(e => e.UtilisateurId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}