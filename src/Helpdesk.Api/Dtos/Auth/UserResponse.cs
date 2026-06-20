namespace Helpdesk.Api.Dtos.Auth;

/// <summary>A user account returned to clients (never carries credentials).</summary>
public record UserResponse(Guid Id, string Email, string DisplayName, string Role);
