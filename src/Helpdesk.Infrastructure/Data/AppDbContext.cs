using Helpdesk.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Infrastructure.Data;

/// <summary>
/// EF Core context for the helpdesk domain. Entity shapes live in the
/// <c>IEntityTypeConfiguration</c> classes under <c>Data/Configurations</c>; this context
/// also stamps <c>CreatedAt</c>/<c>UpdatedAt</c> automatically on save.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Draft> Drafts => Set<Draft>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyTimestamps();
        return base.SaveChanges();
    }

    /// <summary>Sets CreatedAt on insert and UpdatedAt on insert/update for timestamped entities.</summary>
    private void ApplyTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;

            switch (entry.Entity)
            {
                case Ticket or Draft:
                    if (entry.State == EntityState.Added)
                        entry.Property(nameof(Ticket.CreatedAt)).CurrentValue = now;
                    entry.Property(nameof(Ticket.UpdatedAt)).CurrentValue = now;
                    break;

                case User when entry.State == EntityState.Added:
                    entry.Property(nameof(User.CreatedAt)).CurrentValue = now;
                    break;
            }
        }
    }
}
