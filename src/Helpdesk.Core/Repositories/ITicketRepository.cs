using Helpdesk.Core.Entities;
using Helpdesk.Core.Enums;

namespace Helpdesk.Core.Repositories;

/// <summary>Persistence contract for <see cref="Ticket"/> aggregates (ticket + its draft).</summary>
public interface ITicketRepository
{
    /// <summary>Loads a ticket with its draft, or null if not found.</summary>
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Looks up a ticket by the inbound Gmail message id, for ingestion dedupe.</summary>
    Task<Ticket?> GetByMessageIdAsync(string messageId, CancellationToken ct = default);

    /// <summary>
    /// Lists tickets newest-first, optionally filtered by state and/or category — the
    /// query backing the agent review queue.
    /// </summary>
    Task<IReadOnlyList<Ticket>> ListAsync(
        TicketState? state = null,
        Category? category = null,
        CancellationToken ct = default);

    /// <summary>Stages a new ticket for insertion. Call <see cref="SaveChangesAsync"/> to persist.</summary>
    Task AddAsync(Ticket ticket, CancellationToken ct = default);

    /// <summary>Persists all pending changes tracked on loaded/added entities.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
