using Helpdesk.Core.Entities;
using Helpdesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Infrastructure.Data;

/// <summary>
/// EF Core context for the helpdesk domain. Inherits <see cref="IdentityUserContext{TUser,TKey}"/>
/// so ASP.NET Core Identity owns the user/credential tables (no role tables — role is a plain
/// column on <see cref="ApplicationUser"/>). Domain entity shapes live in the
/// <c>IEntityTypeConfiguration</c> classes under <c>Data/Configurations</c>; this context
/// also stamps <c>CreatedAt</c>/<c>UpdatedAt</c> automatically on save.
/// </summary>
public class AppDbContext : IdentityUserContext<ApplicationUser, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Draft> Drafts => Set<Draft>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Identity's own entity configuration must run first.
        base.OnModelCreating(modelBuilder);
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

                case ApplicationUser when entry.State == EntityState.Added:
                    entry.Property(nameof(ApplicationUser.CreatedAt)).CurrentValue = now;
                    break;
            }
        }
    }
}
