using CesiZen.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.API.Data.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // ?? Roles ?????????????????????????????????????????????
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role { RoleId = 1, Nom = "Administrateur" },
                    new Role { RoleId = 2, Nom = "Utilisateur" }
                );
                await context.SaveChangesAsync();
            }

            // ?? Compte Admin ??????????????????????????????????????
            if (!await context.Utilisateurs.AnyAsync(u => u.Email == "admin@cesizen.fr"))
            {
                context.Utilisateurs.Add(new Utilisateur
                {
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

            // ?? Menu par défaut ???????????????????????????????????
            if (!await context.Menus.AnyAsync())
            {
                context.Menus.AddRange(
                    new Menu { MenuId = 1, Libelle = "Accueil", Ordre = 1 },
                    new Menu { MenuId = 2, Libelle = "Santé mentale", Ordre = 2 },
                    new Menu { MenuId = 3, Libelle = "Ressources", Ordre = 3 }
                );
                await context.SaveChangesAsync();
            }

            // ?? Page d'accueil ????????????????????????????????????
            if (!await context.PagesInfo.AnyAsync())
            {
                var admin = await context.Utilisateurs
                    .FirstAsync(u => u.Email == "admin@cesizen.fr");

                context.PagesInfo.Add(new PageInfo
                {
                    Titre = "Bienvenue sur CESIZen",
                    Contenu = "CESIZen est votre application de santé mentale. " +
                              "Découvrez nos outils pour gérer votre stress au quotidien.",
                    Slug = "accueil",
                    IsPublic = true,
                    MenuId = 1,
                    UtilisateurId = admin.UtilisateurId,
                    UpdatedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }

            // ?? Exercices de respiration ??????????????????????????
            if (!await context.ExercicesRespiration.AnyAsync())
            {
                var admin = await context.Utilisateurs
                    .FirstAsync(u => u.Email == "admin@cesizen.fr");

                context.ExercicesRespiration.AddRange(
                    new ExerciceRespiration
                    {
                        Nom = "748",
                        DureeInspiration = 7,
                        DureeApnee = 4,
                        DureeExpiration = 8,
                        Actif = true,
                        UtilisateurId = admin.UtilisateurId
                    },
                    new ExerciceRespiration
                    {
                        Nom = "55",
                        DureeInspiration = 5,
                        DureeApnee = 0,
                        DureeExpiration = 5,
                        Actif = true,
                        UtilisateurId = admin.UtilisateurId
                    },
                    new ExerciceRespiration
                    {
                        Nom = "46",
                        DureeInspiration = 4,
                        DureeApnee = 0,
                        DureeExpiration = 6,
                        Actif = true,
                        UtilisateurId = admin.UtilisateurId
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}