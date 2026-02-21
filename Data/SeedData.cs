using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevOpsReleasePortal.Data;

public static class SeedData
{
    public static readonly string[] Roles = ["Developer", "DevOps", "Tester", "Manager", "BA"];

    public static async Task InitializeAsync(IServiceProvider services, IWebHostEnvironment environment, IConfiguration configuration)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                {
                    logger.LogWarning("Failed to create role {Role}: {Errors}", role, string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        var enableDevUsers = configuration.GetValue<bool?>("Seed:EnableDevelopmentUsers") ?? true;
        if (!environment.IsDevelopment() || !enableDevUsers)
        {
            return;
        }

        await EnsureUserAsync(userManager, logger, "devops@example.com", "DevOps@123", "DevOps");
        await EnsureUserAsync(userManager, logger, "dev@example.com", "Dev@123", "Developer");
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        ILogger logger,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                logger.LogWarning("Failed to create user {Email}: {Errors}", email, string.Join("; ", createResult.Errors.Select(e => e.Description)));
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                logger.LogWarning("Failed to add user {Email} to role {Role}: {Errors}", email, role, string.Join("; ", roleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
