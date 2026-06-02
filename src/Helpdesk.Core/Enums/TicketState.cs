namespace Helpdesk.Core.Enums;

/// <summary>
/// Minimal ticket lifecycle for the MVP: a ticket waits for an agent to review the
/// drafted reply, then becomes Sent once that reply goes back to the sender.
/// </summary>
public enum TicketState
{
    NeedsReview,
    Sent
}
