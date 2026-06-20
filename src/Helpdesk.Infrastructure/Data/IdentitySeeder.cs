using Helpdesk.Core.Enums;
using Helpdesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helpdesk.Infrastructure.Data;

/// <summary>
/// Bootstraps the first Admin (the project owner) so someone can log in on a fresh
/// database. Credentials come from the <c>Bootstrap</c> configuration section
/// (<c>AdminEmail</c>/<c>AdminPassword</c>), kept in a non-committed source. No-ops when
/// nothing is configured or the admin already exists. The password must satisfy the
/// Identity password policy.
/// </summary>
public static class IdentitySeeder
{
    public static async Task EnsureBootstrapAdminAsync(IServiceProvider services)
    {
        var config = services.GetRequiredService<IConfiguration>();
        var email = config["Bootstrap:AdminEmail"];
        var password = config["Bootstrap:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = config["Bootstrap:AdminDisplayName"] ?? "Administrator",
            Role = UserRole.Admin,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new InvalidOperationException($"Failed to create bootstrap admin '{email}'. {errors}");
        }
    }
}
