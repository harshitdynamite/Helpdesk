using Helpdesk.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk.Infrastructure.Data.Configurations;

public class DraftConfiguration : IEntityTypeConfiguration<Draft>
{
    public void Configure(EntityTypeBuilder<Draft> builder)
    {
        builder.ToTable("drafts");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Body).IsRequired();
        builder.Property(d => d.IsAiGenerated).IsRequired();

        // The Ticket ↔ Draft relationship (and its unique FK) is configured on the
        // ticket side; here we only seed the draft rows.
        builder.HasData(SeedData.Drafts());
    }
}
