using Helpdesk.Core.Enums;

namespace Helpdesk.Core.Entities;

/// <summary>
/// A support request created from an inbound email. Carries the original message, the
/// AI-produced classification/summary, and the workflow state. The drafted reply lives
/// on the associated <see cref="Draft"/>.
/// </summary>
public class Ticket
{
    public Guid Id { get; set; }

    // ----- Original email -----
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public required string SenderEmail { get; set; }
    public string? SenderName { get; set; }

    /// <summary>Gmail thread id, so the reply lands in the original conversation.</summary>
    public string? ThreadId { get; set; }

    /// <summary>Gmail message id of the inbound mail; used to dedupe on ingestion.</summary>
    public string? MessageId { get; set; }

    // ----- AI output (null until classified/summarized in Phase 5) -----
    public Category? Category { get; set; }
    public string? Summary { get; set; }

    // ----- Workflow -----
    public TicketState State { get; set; } = TicketState.NeedsReview;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>Set when the reply is sent and the ticket moves to <see cref="TicketState.Sent"/>.</summary>
    public DateTime? SentAt { get; set; }

    /// <summary>The AI-drafted, agent-editable reply. Null until a draft has been generated.</summary>
    public Draft? Draft { get; set; }
}
