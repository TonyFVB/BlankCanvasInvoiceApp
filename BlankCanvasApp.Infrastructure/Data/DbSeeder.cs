using BlankCanvasApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BlankCanvasApp.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<BcDContext>>();

            // ── 1. Crear roles y asignar permisos como claims ────────
            foreach (var roleName in AppRoles.All)
            {
                IdentityRole? role;

                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var createResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!createResult.Succeeded)
                    {
                        logger.LogError("Error creando rol '{Role}': {Errors}",
                            roleName, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                        continue;
                    }
                    logger.LogInformation("Rol '{Role}' creado.", roleName);
                }

                role = await roleManager.FindByNameAsync(roleName);
                if (role == null) continue;

                // Sincroniza claims — agrega solo los que faltan
                var existingClaims = await roleManager.GetClaimsAsync(role);
                var existingValues = existingClaims
                    .Where(c => c.Type == "permission")
                    .Select(c => c.Value)
                    .ToHashSet();

                if (AppPermissions.PorRol.TryGetValue(roleName, out var permissions))
                {
                    foreach (var permission in permissions)
                    {
                        if (!existingValues.Contains(permission))
                        {
                            await roleManager.AddClaimAsync(role, new Claim("permission", permission));
                            logger.LogInformation("Permiso '{Permission}' → rol '{Role}'.", permission, roleName);
                        }
                    }
                }
            }

            // ── 2. Crear usuario Admin por defecto ───────────────────
            const string adminEmail = "admin@blackcanvas.com";
            const string adminPassword = "Admin@12345"; // ← cambiar en producción

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FirstName = "Super",
                    LastName = "Admin",
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
                    logger.LogInformation("Admin creado: {Email}", adminEmail);
                }
                else
                {
                    logger.LogError("Error creando Admin: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}