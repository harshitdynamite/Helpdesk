---
name: endpoint-authorization-map
description: Every API endpoint, its HTTP method, and authorization requirement — confirmed from source code
metadata:
  type: project
---

## Endpoint Authorization Map (confirmed 2026-06-20)

| Endpoint | Method | Authorization | Notes |
|---|---|---|---|
| `GET /api/health` | GET | None (anonymous) | Liveness probe — intentionally public |
| `POST /api/auth/login` | POST | None (anonymous) | Credential exchange — must be public |
| `GET /api/auth/me` | GET | `[Authorize]` (any authenticated user) | Token echo/probe |
| `POST /api/auth/agents` | POST | `[Authorize(Roles = "Admin")]` | Admin-only agent provisioning |

## Key Observations

- No global `[Authorize]` filter on the controller level in `AuthController` — each action is decorated individually. This is correct but means new actions must opt-in.
- `HealthController` has no `[Authorize]` — intentional (liveness probe).
- No `TicketsController` exists yet (planned Phase 2) — IDOR risks will need review when built.
- `POST /api/auth/agents` correctly restricts to `Admin` role. The role string used is `nameof(UserRole.Admin)` = `"Admin"`, which matches what `TokenService` emits as `ClaimTypes.Role`.

## Frontend Route Guards (client-side only — not a security boundary)

| Route | Guard | Role Required |
|---|---|---|
| `/` | `RequireAuth` | Any authenticated session |
| `/users` | `RequireRole` | `"Admin"` |
| `/login` | None | Public |

Note: These are UI-only guards backed by localStorage session. The backend enforces real authorization; the frontend guards are UX only.
