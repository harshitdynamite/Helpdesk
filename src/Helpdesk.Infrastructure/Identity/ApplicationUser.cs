using Helpdesk.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace Helpdesk.Infrastructure.Identity;

/// <summary>
/// The Identity-backed user. Email/password, normalized lookups, hashing, etc. come from
/// <see cref="IdentityUser{TKey}"/>; we add the helpdesk-specific fields. Role is kept as a
/// plain <see cref="UserRole"/> column (Admin/Agent) rather than Identity's role tables —
/// it is emitted as a claim when a token is issued.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public required string DisplayName { get; set; }

    public UserRole Role { get; set; } = UserRole.Agent;

    public DateTime CreatedAt { get; set; }
}
