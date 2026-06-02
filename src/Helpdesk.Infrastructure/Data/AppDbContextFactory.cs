using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Helpdesk.Infrastructure.Data;

/// <summary>
/// Lets the EF Core CLI (migrations, database update) construct an <see cref="AppDbContext"/>
/// before the API's DI/composition root exists (that wiring arrives in Phase 2). The
/// connection string comes from the <c>HELPDESK_DB</c> environment variable, falling back
/// to a local default — only the design-time tools use this; the running app supplies its
/// own configured connection string later.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=helpdesk;Username=helpdesk;Password=helpdesk";

    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("HELPDESK_DB") ?? DefaultConnectionString;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
