namespace Helpdesk.Api.Dtos.Auth;

/// <summary>Successful login result: the bearer token and basic profile for the client.</summary>
public record LoginResponse(
    string Token,
    string Email,
    string DisplayName,
    string Role,
    DateTime ExpiresAtUtc);
