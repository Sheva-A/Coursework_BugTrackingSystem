using Microsoft.AspNetCore.Identity;
using BugTrackingSystem.Models;

namespace BugTrackingSystem.Data
{
    /// <summary>
    /// Ініціалізатор бази даних: створює системні ролі та адміністратора за замовчуванням.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Створює всі ролі системи та стандартного адміністратора, якщо вони ще відсутні.
        /// Викликається при старті застосунку.
        /// </summary>
        /// <param name="serviceProvider">Провайдер сервісів DI.</param>
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Створити всі ролі, якщо їх ще немає
            foreach (var roleName in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Створити адміністратора за замовчуванням
            const string adminEmail = "admin@bugtracker.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin123!");

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(newAdmin, AppRoles.Admin);
            }
        }
    }
}