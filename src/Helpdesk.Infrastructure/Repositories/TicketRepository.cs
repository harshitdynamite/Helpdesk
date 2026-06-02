using Helpdesk.Core.Entities;
using Helpdesk.Core.Enums;
using Helpdesk.Core.Repositories;
using Helpdesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _db;

    public TicketRepository(AppDbContext db) => _db = db;

    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Tickets
            .Include(t => t.Draft)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<Ticket?> GetByMessageIdAsync(string messageId, CancellationToken ct = default) =>
        _db.Tickets
            .FirstOrDefaultAsync(t => t.MessageId == messageId, ct);

    public async Task<IReadOnlyList<Ticket>> ListAsync(
        TicketState? state = null,
        Category? category = null,
        CancellationToken ct = default)
    {
        var query = _db.Tickets
            .Include(t => t.Draft)
            .AsQueryable();

        if (state is not null)
            query = query.Where(t => t.State == state);

        if (category is not null)
            query = query.Where(t => t.Category == category);

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Ticket ticket, CancellationToken ct = default) =>
        await _db.Tickets.AddAsync(ticket, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}
