---
name: frontend-token-storage
description: Frontend JWT storage mechanism, token handling, and API proxy configuration — confirmed from source
metadata:
  type: project
---

## Token Storage (confirmed 2026-06-20)

- **Storage**: `localStorage` under key `"helpdesk.session"`
- **What is stored**: Full `Session` object — `token` (JWT), `email`, `displayName`, `role`, `expiresAtUtc`
- **Location**: `frontend/src/auth/AuthContext.tsx`

### Risk: localStorage vs httpOnly cookie

localStorage is accessible to any JavaScript on the page, making it vulnerable to XSS. An httpOnly cookie would prevent JS access. This is a known trade-off for SPAs but is the dominant XSS escalation path: if any XSS fires, the attacker gets the JWT.

**Mitigating factor**: No XSS vectors currently exist in rendered content (no dangerouslySetInnerHTML, no HTML email rendering yet). Risk will escalate significantly when ticket/email body rendering is added.

## Client-Side Expiry Check

`loadStoredSession()` checks `expiresAtUtc` against `Date.now()` and discards expired sessions — correct UX behavior. This is NOT a security control (client-side only).

## API Proxy

- Bun dev server in `frontend/src/index.ts` forwards all `/api/*` to `http://localhost:5155`
- No auth headers added by the proxy — token is sent by the React app via `authHeader()` utility in `frontend/src/api/client.ts`
- Proxy is dev-only; production deployment target TBD

## No Rate Limiting on Login

`POST /api/auth/login` has no rate limiting, account lockout, or CAPTCHA. Identity's lockout is not explicitly configured (not verified whether `AddIdentityCore` defaults enable lockout).
