# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Helpdesk is an AI-assisted support-ticket MVP. The core loop: ingest support emails from Gmail → AI classifies + summarizes each ticket → AI drafts a reply grounded in a hardcoded knowledge base → a human agent reviews/edits the draft in a queue and sends it back in the original email thread. **No reply is ever auto-sent** — a human approves every one. See `project-scope.md`, `tech-stack.md`, and `implementation-plan.md` for the full design and phased plan.

Status: in active build. Phase 1 (domain entities + EF Core/Postgres persistence with migrations) and Phase 3 (auth — ASP.NET Core Identity with email/password + JWT bearer) are in place; the backend has `HealthController` and `AuthController`. Phase 2 (the `TicketsController` HTTP surface) and the frontend agent UI are still mostly the planned **target**, not yet built. Consult `implementation-plan.md` for the build order and which phase a task belongs to.

## Commands

Backend (.NET 10, run from repo root):

```bash
# Run the API (http profile → http://localhost:5155, Swagger/OpenAPI in Development)
dotnet run --project src/Helpdesk.Api --launch-profile http

# Build (building the Api project compiles Core + Infrastructure via project refs)
dotnet build src/Helpdesk.Api
```

There is **no `.sln` file** yet even though the docs reference `Helpdesk.sln`. Target individual project paths with `dotnet` commands, or create the solution if a task calls for it.

Frontend (Bun + React, run from `frontend/`):

```bash
bun install        # first time
bun dev            # dev server with HMR → http://localhost:3000
bun run build      # production bundle to dist/
```

Run both together: start the backend on 5155 first, then `bun dev`. The frontend dev server (`frontend/src/index.ts`) proxies all `/api/*` requests to `http://localhost:5155`, so the browser only ever talks to port 3000. The proxy target and listen port are env-overridable (`API_PROXY_TARGET`, `PORT`), defaulting to those dev values.

### Testing — Playwright E2E (run from `frontend/`)

```bash
bun run e2e            # full-stack E2E run (Playwright orchestrates everything)
bun run e2e:ui         # same, in Playwright's interactive UI mode
bun run e2e:install    # one-time: download Playwright browser binaries
```

There is **no unit/integration test runner yet** (no xUnit project, no Vitest). The only tests are **Playwright E2E** under `frontend/e2e/` (config: `frontend/playwright.config.ts`). Key points:

- **Full orchestration, fully isolated.** `bun run e2e` starts its own API + frontend on **dedicated ports** (API `:5200`, frontend `:3100` — overridable via `E2E_API_PORT` / `E2E_WEB_PORT`) pointed at a **separate `helpdesk_e2e` PostgreSQL database**. So E2E runs **alongside** the dev servers without colliding.
- **The `Testing` environment.** The E2E API runs with `ASPNETCORE_ENVIRONMENT=Testing`; `Program.cs` treats `Testing` like `Development` for seeding so the bootstrap admin (`admin@e2e.test` / `Admin!2345`) and sample data are created in the test DB. The test connection string, JWT key, and bootstrap creds are injected as env vars by `playwright.config.ts` (nothing test-specific is committed to `appsettings*.json`).
- **DB lifecycle.** `frontend/e2e/start-api.ts` migrates the test DB (via Infrastructure's `AppDbContextFactory` + the `HELPDESK_DB` env var) **before** the API serves, then runs the API. It uses `--no-build` to avoid fighting a running dev `dotnet run` over the shared `bin/` (which otherwise causes `MSB3027` file locks); it builds once only if no artifacts exist. There is currently **no per-run DB reset** — the test DB persists between runs (seeders are idempotent).
- **Local DB credentials** live in a gitignored `frontend/.env.e2e` (copy `frontend/.env.e2e.example`; set `E2E_DB_PASSWORD` to your local Postgres password).

No test files exist yet — `frontend/e2e/` holds only the harness. Use the **playwright-e2e-author** agent to write specs for user-facing flows (login, review queue, review screen).

## Architecture

Two independently deployable halves talking over REST/JSON.

### Backend — Clean Architecture, three projects

```
Helpdesk.Api  ──▶  Helpdesk.Core  ◀──  Helpdesk.Infrastructure
```

- **Helpdesk.Core** — the domain, **depends on nothing**. Entities (`Ticket`, `Reply`/`Draft`), enums (`Category`, `TicketState` = NeedsReview → Sent, `UserRole` = Admin/Agent), and the interfaces everyone else implements: repositories (`ITicketRepository`) and service contracts (`ITokenService`, `IEmailClient`, `IAiService`, `IKnowledgeBase`). The user identity is an Identity concern and lives in Infrastructure, not here.
- **Helpdesk.Infrastructure** — implementations and all I/O: EF Core `AppDbContext` (an Identity `IdentityUserContext`) + migrations (Npgsql/PostgreSQL), repository implementations, ASP.NET Core Identity (`ApplicationUser : IdentityUser<Guid>`, `UserManager`, JWT `TokenService`), and the external integrations (Gmail client, LLM service, hardcoded knowledge base). Depends on Core only.
- **Helpdesk.Api** — the HTTP surface and composition root: attribute-routed **controllers** (not Minimal API), DTOs + mapping, validation, the JWT bearer auth scheme + role enforcement, and DI wiring in `Program.cs`. References Infrastructure **only** to register implementations at startup.

Key conventions:
- **Controllers, not Minimal API.** Routes are `api/[controller]` (see `HealthController`).
- **Entities never cross the API boundary** — controllers accept and return DTOs.
- **External integrations are built behind interfaces and mocked first** (`MockEmailClient`, `MockAiService`). This lets the entire ingest→draft→send loop run end-to-end before any real Gmail/LLM credentials exist; the real implementations are swapped in behind the same interface later.
- **Auth is stateless JWT.** `AuthController` (`POST /api/auth/login`) verifies the password via Identity's `UserManager` and returns a signed JWT carrying the user's `UserRole` as a role claim; protected endpoints use `[Authorize]` / `[Authorize(Roles = …)]`. No cookies, no `SignInManager`, no login UI. The signing key and bootstrap-admin credentials come from config/secrets (`Jwt:SigningKey`, `Bootstrap:*` in `appsettings.Development.local.json`), never committed.

### Frontend — React + TypeScript (Bun)

- `frontend/src/index.ts` is the Bun server entry: serves the React app and proxies `/api/*` to the backend (the integration seam between the two halves).
- `frontend/src/frontend.tsx` → `App.tsx` is the React entry. The agent UI it will grow into: a ticket **review queue** (list with filter/sort) and a **review screen** (original email, AI summary + category, editable draft, send).
- Bun is the package manager, dev server, and bundler — single toolchain, no separate Vite/webpack/npm.

## Decisions still open (don't assume)

These are intentionally undecided in the planning docs — confirm before building on them: LLM provider (behind `IAiService`), Gmail access method (mailbox OAuth vs. service account), and deployment target. The first deployed admin is the project owner (bootstrap), who can then create agent logins.

**Resolved:** auth mechanism — **email/password via ASP.NET Core Identity with JWT bearer tokens** (`ApplicationUser.Role` = Admin/Agent). See the auth convention above.
