---
name: project-auth-e2e-surface
description: Auth E2E surface — routes, selectors, localStorage key, API shapes, and test file location for the Helpdesk auth system
metadata:
  type: project
---

# Auth E2E surface

**Test file:** `frontend/e2e/auth.spec.ts` (34 tests, all passing as of 2026-06-21)

## Routes (from App.tsx)
- `/login` — LoginPage (no auth required)
- `/` — HomePage wrapped in RequireAuth → redirects to /login if unauthenticated
- `/users` — UsersPage wrapped in RequireRole role="Admin" → redirects to /login if unauthenticated, to / if wrong role
- `*` (catch-all) → Navigate to / → RequireAuth → /login if unauthenticated

## localStorage session
- **Key:** `helpdesk.session`
- **Shape:** `{ token: string, email: string, displayName: string, role: string, expiresAtUtc: string }`
- **Expiry logic:** `loadStoredSession()` in AuthContext.tsx discards if `new Date(expiresAtUtc).getTime() <= Date.now()`; also discards on JSON.parse throw. Both paths call `localStorage.removeItem(key)`.

## Seeded test credentials
- Admin: `admin@e2e.test` / `Admin!2345`, displayName "E2E Admin", role "Admin"
- Agent: `agent-auth-test@e2e.test` / `Agent!2345`, displayName "Auth Test Agent", role "Agent" — provisioned idempotently in `beforeAll` via `POST /api/auth/agents`

## API endpoints used in tests
- `POST /api/auth/login` → `{ token, email, displayName, role, expiresAtUtc }` (200) or 401 Unauthorized
- `POST /api/auth/agents` (Admin JWT required) → 201 Created or 409 Conflict

## Key selectors discovered
- Login form title: `getByText("Helpdesk")` — CardTitle renders as `<div>`, NOT a heading element
- Login form subtitle: `getByText("Sign in to your account")` — CardDescription is also a `<div>`
- Email input: `getByLabel("Email")` (htmlFor="email", type="email")
- Password input: `getByLabel("Password")` (htmlFor="password", type="password")
- Submit button: `getByRole("button", { name: "Sign in" })` or `locator('button[type="submit"]')` (stable through text change)
- Loading state text: "Signing in…" (with ellipsis character, not "...")
- Invalid credentials error: `getByText("Invalid email or password.")`
- Zod email error: `getByText("Enter a valid email address")`
- Zod password error: `getByText("Password is required")`
- Server 500 error: matches `/Login failed \(500\)/`
- Network error: `getByText("Could not reach the server. Is the backend running?")`
- NavBar: `page.locator("nav")` — contains displayName span, Users link, Sign out button
- Users link (admin only): `nav.getByRole("link", { name: "Users" })`
- Sign out: `getByRole("button", { name: "Sign out" })`
- Home page heading: `getByRole("heading", { name: /Welcome, E2E Admin/i })` (HomePage renders `<h1>`)
- Users page heading: `getByRole("heading", { name: "Users" })` (UsersPage renders `<h1>`)

## Critical implementation notes

### addInitScript pitfall
`context.addInitScript()` persists for the lifetime of the BrowserContext and re-runs on EVERY navigation, including navigations that happen AFTER a logout. Do NOT use it in tests that sign in then sign out in the same context and expect the post-logout page.goto("/") to redirect to /login. Fix: use a fresh `browser.newContext()` for the post-logout verification, or use the UI login flow (no addInitScript) and open a second page in the same context after logout.

### Submit button locator during loading state
After clicking "Sign in", the button text changes to "Signing in…" — locating by `getByRole("button", { name: /Sign in/i })` fails because the name no longer matches. Use `locator('button[type="submit"]')` stored BEFORE click instead.

### Non-object JSON in localStorage
A string like `'"just-a-string"'` is valid JSON but casts to a string. `loadStoredSession` reads `.expiresAtUtc` which is `undefined` on a string. `new Date(undefined).getTime()` is NaN; `NaN <= Date.now()` is `false`, so the "session" is returned without expiry-clearing. The app then considers the user authenticated but with a broken session object (no .role). This is an acceptable edge case — the app doesn't crash.

**Why:** session management via `loadStoredSession` in `frontend/src/auth/AuthContext.tsx`
**How to apply:** understand what the expiry-check catches and what it doesn't when writing session edge-case tests.
