---
name: auth-jwt-config
description: Auth and JWT configuration details — signing key sourcing, token parameters, validation config, and bootstrap admin handling
metadata:
  type: project
---

## JWT Configuration (confirmed from code, 2026-06-20)

- **Algorithm**: HMAC-SHA256 (`SecurityAlgorithms.HmacSha256`) — symmetric, single-key
- **Token expiry**: 60 minutes default (`JwtOptions.ExpiryMinutes`), set in `appsettings.json`
- **Signing key source**: `Jwt:SigningKey` config key — empty string in committed `appsettings.json`; real value lives in gitignored `appsettings.Development.local.json` (confirmed not in git history). No hardcoded key in source.
- **Issuer**: `"Helpdesk.Api"` (committed `appsettings.json`)
- **Audience**: `"Helpdesk.Client"` (committed `appsettings.json`)
- **Claims emitted**: `sub` (userId Guid), `email`, `ClaimTypes.Role` (Admin/Agent string), `jti` (UUID per token)
- **ClockSkew**: 1 minute (set explicitly in `Program.cs` — tighter than the 5-minute ASP.NET Core default)
- **Validation flags**: `ValidateIssuer=true`, `ValidateAudience=true`, `ValidateLifetime=true`, `ValidateIssuerSigningKey=true` — all four enabled
- **Location**: `TokenService.cs` (Infrastructure/Auth), `Program.cs` (Api), `JwtOptions.cs` (Infrastructure/Auth)

## Key Security Concern — Empty SigningKey Fallback

`JwtOptions.SigningKey` defaults to `string.Empty`. If the secret is not provided (missing env/local file), `TokenService` will sign with an empty key — a zero-entropy key that any attacker could forge. There is **no startup validation** that fails fast when `SigningKey` is empty or too short.

**Remediation needed**: Add startup guard in `Program.cs` or `AddInfrastructure` that throws `InvalidOperationException` if `jwt.SigningKey` is null/empty/too short (< 32 chars recommended for HS256).

## Bootstrap Admin Credentials

- Source: `Bootstrap:AdminEmail`, `Bootstrap:AdminPassword`, `Bootstrap:AdminDisplayName` config keys
- Committed `appsettings.json` has all three as empty strings — correct
- Real values live in gitignored `appsettings.Development.local.json` — confirmed NOT in git history
- `IdentitySeeder.EnsureBootstrapAdminAsync` called only in Development environment — correct
- Password goes through `UserManager.CreateAsync` (Identity hashing, bcrypt-derived PBKDF2) — correct

## Password Verification

- Uses `UserManager.CheckPasswordAsync` — Identity's constant-time hash comparison — correct
- No manual/weak comparison anywhere in the flow

## Role Handling

- `ApplicationUser.Role` is a plain `UserRole` enum column (not Identity role tables)
- Role is emitted as `ClaimTypes.Role` claim at token issuance time
- Role is NOT re-read from DB on each request — if a user's role changes, existing tokens still carry the old role until they expire
