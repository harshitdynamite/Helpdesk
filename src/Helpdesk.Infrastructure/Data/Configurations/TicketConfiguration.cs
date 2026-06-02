using Helpdesk.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk.Infrastructure.Data.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Subject).HasMaxLength(500).IsRequired();
        builder.Property(t => t.Body).IsRequired();
        builder.Property(t => t.SenderEmail).HasMaxLength(320).IsRequired();
        builder.Property(t => t.SenderName).HasMaxLength(320);
        builder.Property(t => t.ThreadId).HasMaxLength(256);
        builder.Property(t => t.MessageId).HasMaxLength(256);
        builder.Property(t => t.Summary).HasMaxLength(2000);

        // Store enums as readable strings rather than ints.
        builder.Property(t => t.Category).HasConversion<string>().HasMaxLength(32);
        builder.Property(t => t.State).HasConversion<string>().HasMaxLength(32).IsRequired();

        // Dedupe inbound mail: a message id, when present, maps to at most one ticket.
        builder.HasIndex(t => t.MessageId)
            .IsUnique()
            .HasFilter("\"MessageId\" IS NOT NULL");

        // Queue queries filter/sort on these.
        builder.HasIndex(t => t.State);
        builder.HasIndex(t => t.CreatedAt);

        // One ticket has at most one draft; deleting the ticket removes its draft.
        builder.HasOne(t => t.Draft)
            .WithOne(d => d.Ticket)
            .HasForeignKey<Draft>(d => d.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // Sample tickets are seeded at runtime in Development only (see DevelopmentDataSeeder),
        // not via HasData, so production databases stay empty.
    }
}
