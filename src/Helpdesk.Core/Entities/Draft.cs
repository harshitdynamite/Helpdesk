namespace Helpdesk.Core.Entities;

/// <summary>
/// The suggested reply for a ticket. The AI generates the initial text; an agent edits
/// <see cref="Body"/> in place before approving it. There is at most one draft per ticket,
/// and its body is the text that gets sent.
/// </summary>
public class Draft
{
    public Guid Id { get; set; }

    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    /// <summary>The reply text. Starts as the AI suggestion; the agent edits this directly.</summary>
    public required string Body { get; set; }

    /// <summary>True while still the unedited AI suggestion; cleared once an agent edits it.</summary>
    public bool IsAiGenerated { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
