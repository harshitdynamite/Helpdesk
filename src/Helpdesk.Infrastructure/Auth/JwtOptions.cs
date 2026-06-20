namespace Helpdesk.Infrastructure.Auth;

/// <summary>
/// Strongly-typed view of the <c>Jwt</c> configuration section. <see cref="SigningKey"/> is
/// a secret and must come from a non-committed source (user-secrets, environment, or
/// <c>appsettings.*.local.json</c>).
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}
