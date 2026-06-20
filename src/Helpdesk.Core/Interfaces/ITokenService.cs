using Helpdesk.Core.Enums;

namespace Helpdesk.Core.Interfaces;

/// <summary>
/// Issues a signed access token for an authenticated user. Defined in Core with
/// primitive inputs only so the API can depend on the contract without referencing the
/// Infrastructure (JWT) implementation. Core stays dependency-free.
/// </summary>
public interface ITokenService
{
    AccessToken CreateToken(Guid userId, string email, UserRole role);
}

/// <summary>A bearer token and the instant it expires (UTC).</summary>
public record AccessToken(string Token, DateTime ExpiresAtUtc);
