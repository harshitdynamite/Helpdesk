namespace Helpdesk.Api.Dtos.Auth;

/// <summary>The authenticated caller's identity, read back from the token claims.</summary>
public record MeResponse(string Id, string Email, string Role);
