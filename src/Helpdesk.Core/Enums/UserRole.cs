namespace Helpdesk.Core.Enums;

/// <summary>
/// Access role. The bootstrap owner is an Admin and can create Agent logins;
/// agents work the review queue.
/// </summary>
public enum UserRole
{
    Admin,
    Agent
}
