using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Infrastructure.Data;

/// <summary>
/// Seeds the sample tickets and their drafts for local development only. The owner user
/// is seeded unconditionally through the migration (see
/// <see cref="Configurations.UserConfiguration"/>) and is intentionally not handled here.
/// Call from the composition root, guarded by the Development environment.
/// Idempotent: it no-ops once any tickets exist.
/// </summary>
public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        if (await db.Tickets.AnyAsync(ct))
            return;

        db.Tickets.AddRange(SeedData.Tickets());
        db.Drafts.AddRange(SeedData.Drafts());
        await db.SaveChangesAsync(ct);
    }
}
