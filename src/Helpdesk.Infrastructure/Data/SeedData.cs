using Helpdesk.Core.Entities;
using Helpdesk.Core.Enums;

namespace Helpdesk.Infrastructure.Data;

/// <summary>
/// Fixed sample rows baked into the initial migration for local development. Ids and
/// timestamps are constant (required by EF <c>HasData</c>) so the seed is deterministic
/// and idempotent across migrations. A couple of tickets simulate the post-AI state
/// (category + summary + draft) so the future review queue has something to show.
/// </summary>
internal static class SeedData
{
    // Stable identifiers, referenced from both the ticket and draft configurations.
    public static readonly Guid OwnerUserId = new("a1000000-0000-0000-0000-000000000001");

    public static readonly Guid Ticket1Id = new("b1000000-0000-0000-0000-000000000001");
    public static readonly Guid Ticket2Id = new("b1000000-0000-0000-0000-000000000002");
    public static readonly Guid Ticket3Id = new("b1000000-0000-0000-0000-000000000003");

    public static readonly Guid Draft1Id = new("c1000000-0000-0000-0000-000000000001");
    public static readonly Guid Draft2Id = new("c1000000-0000-0000-0000-000000000002");

    private static readonly DateTime Seeded = new(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc);

    public static User Owner() => new()
    {
        Id = OwnerUserId,
        Email = "harshit.agarwal.se@gmail.com",
        DisplayName = "Harshit Agarwal",
        Role = UserRole.Admin,
        CreatedAt = Seeded
    };

    public static IEnumerable<Ticket> Tickets() =>
    [
        new()
        {
            Id = Ticket1Id,
            Subject = "Can't log in to my account",
            Body = "Hi, I've tried resetting my password twice but I still get an " +
                   "\"invalid credentials\" error when I sign in. Can you help? — Priya",
            SenderEmail = "priya.student@example.edu",
            SenderName = "Priya Student",
            ThreadId = "thread-0001",
            MessageId = "msg-0001",
            Category = Category.AccountLogin,
            Summary = "Student cannot sign in; password reset did not resolve an invalid-credentials error.",
            State = TicketState.NeedsReview,
            CreatedAt = Seeded,
            UpdatedAt = Seeded
        },
        new()
        {
            Id = Ticket2Id,
            Subject = "Was I charged twice this month?",
            Body = "I see two charges for the monthly plan on my card statement. " +
                   "Could you check and refund the duplicate if it's a mistake? Thanks, Sam",
            SenderEmail = "sam.buyer@example.com",
            SenderName = "Sam Buyer",
            ThreadId = "thread-0002",
            MessageId = "msg-0002",
            Category = Category.Billing,
            Summary = "Customer reports a possible duplicate monthly charge and requests a refund of the duplicate.",
            State = TicketState.NeedsReview,
            CreatedAt = Seeded,
            UpdatedAt = Seeded
        },
        // Freshly ingested: no AI output or draft yet (simulates the pre-Phase-5 state).
        new()
        {
            Id = Ticket3Id,
            Subject = "Video lectures won't play on Safari",
            Body = "The course videos just show a black screen in Safari but work fine in " +
                   "Chrome. Is there a fix? — Lee",
            SenderEmail = "lee.learner@example.edu",
            SenderName = "Lee Learner",
            ThreadId = "thread-0003",
            MessageId = "msg-0003",
            State = TicketState.NeedsReview,
            CreatedAt = Seeded,
            UpdatedAt = Seeded
        }
    ];

    public static IEnumerable<Draft> Drafts() =>
    [
        new()
        {
            Id = Draft1Id,
            TicketId = Ticket1Id,
            Body = "Hi Priya,\n\nSorry for the trouble signing in. Let's get you back in:\n" +
                   "1) Use the \"Forgot password\" link and make sure you open the reset email " +
                   "within 30 minutes.\n2) Clear your browser cache or try an incognito window.\n" +
                   "If it still fails, reply here and we'll reset it manually.\n\nBest,\nSupport",
            IsAiGenerated = true,
            CreatedAt = Seeded,
            UpdatedAt = Seeded
        },
        new()
        {
            Id = Draft2Id,
            TicketId = Ticket2Id,
            Body = "Hi Sam,\n\nThanks for flagging this. I can see two charges for your monthly " +
                   "plan and have queued a refund for the duplicate — it should appear in 5–7 " +
                   "business days. You won't be charged twice going forward.\n\nBest,\nSupport",
            IsAiGenerated = true,
            CreatedAt = Seeded,
            UpdatedAt = Seeded
        }
    ];
}
