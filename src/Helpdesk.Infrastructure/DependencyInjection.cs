using Helpdesk.Core.Interfaces;
using Helpdesk.Core.Repositories;
using Helpdesk.Infrastructure.Auth;
using Helpdesk.Infrastructure.Data;
using Helpdesk.Infrastructure.Identity;
using Helpdesk.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helpdesk.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer. The API calls
/// <see cref="AddInfrastructure"/> from <c>Program.cs</c> to register EF Core and the
/// repository implementations without taking a direct dependency on any of them.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Connection string 'Default' was not found. Set ConnectionStrings:Default " +
                "in appsettings (or appsettings.Development.local.json for local secrets).");

        // EF Core registers AppDbContext as scoped by default.
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        // ASP.NET Core Identity: password hashing + UserManager over the EF stores. No
        // SignInManager/cookies — the API authenticates statelessly with JWTs.
        services.AddIdentityCore<ApplicationUser>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<ITicketRepository, TicketRepository>();

        return services;
    }
}
