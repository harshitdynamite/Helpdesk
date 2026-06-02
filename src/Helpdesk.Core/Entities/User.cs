using Helpdesk.Core.Enums;

namespace Helpdesk.Core.Entities;

/// <summary>
/// A person who can sign in and work tickets. Auth credentials are added in Phase 3;
/// for now the entity carries identity and role only.
/// </summary>
public class User
{
    public Guid Id { get; set; }

    public required string Email { get; set; }
    public required string DisplayName { get; set; }

    public UserRole Role { get; set; } = UserRole.Agent;

    public DateTime CreatedAt { get; set; }
}
