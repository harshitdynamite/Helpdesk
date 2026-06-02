# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Helpdesk is an AI-assisted support-ticket MVP. The core loop: ingest support emails from Gmail → AI classifies + summarizes each ticket → AI drafts a reply grounded in a hardcoded knowledge base → a human agent reviews/edits the draft in a queue and sends it back in the original email thread. **No reply is ever auto-sent** — a human approves every one. See `project-scope.md`, `tech-stack.md`, and `implementation-plan.md` for the full design and phased plan.

Status: early scaffold. The backend currently has only a `HealthController` and the default `Program.cs`; `Helpdesk.Core` and `Helpdesk.Infrastructure` are empty project shells (no EF Core packages, entities, or DbContext yet). The frontend is the stock Bun + React template wired to call the API. Most of the architecture below is the **target** described in the planning docs, not yet built — consult `implementation-plan.md` for the build order and which phase a task belongs to.

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

Run both together: start the backend on 5155 first, then `bun dev`. The frontend dev server (`frontend/src/index.ts`) proxies all `/api/*` requests to `http://localhost:5155`, so the browser only ever talks to port 3000.

No test runner or linter is configured yet.

## Architecture

Two independently deployable halves talking over REST/JSON.

### Backend — Clean Architecture, three projects

```
Helpdesk.Api  ──▶  Helpdesk.Core  ◀──  Helpdesk.Infrastructure
```

- **Helpdesk.Core** — the domain, **depends on nothing**. Entities (`Ticket`, `User`, `Reply`/`Draft`), enums (`Category`, `TicketState` = NeedsReview → Sent), and the interfaces everyone else implements: repositories (`ITicketRepository`, `IUserRepository`) and service contracts (`IEmailClient`, `IAiService`, `IKnowledgeBase`).
- **Helpdesk.Infrastructure** — implementations and all I/O: EF Core `AppDbContext` + migrations (Npgsql/PostgreSQL), repository implementations, and the external integrations (Gmail client, LLM service, hardcoded knowledge base). Depends on Core only.
- **Helpdesk.Api** — the HTTP surface and composition root: attribute-routed **controllers** (not Minimal API), DTOs + mapping, validation, role enforcement, and DI wiring in `Program.cs`. References Infrastructure **only** to register implementations at startup.

Key conventions:
- **Controllers, not Minimal API.** Routes are `api/[controller]` (see `HealthController`).
- **Entities never cross the API boundary** — controllers accept and return DTOs.
- **External integrations are built behind interfaces and mocked first** (`MockEmailClient`, `MockAiService`). This lets the entire ingest→draft→send loop run end-to-end before any real Gmail/LLM credentials exist; the real implementations are swapped in behind the same interface later.

### Frontend — React + TypeScript (Bun)

- `frontend/src/index.ts` is the Bun server entry: serves the React app and proxies `/api/*` to the backend (the integration seam between the two halves).
- `frontend/src/frontend.tsx` → `App.tsx` is the React entry. The agent UI it will grow into: a ticket **review queue** (list with filter/sort) and a **review screen** (original email, AI summary + category, editable draft, send).
- Bun is the package manager, dev server, and bundler — single toolchain, no separate Vite/webpack/npm.

## Decisions still open (don't assume)

These are intentionally undecided in the planning docs — confirm before building on them: LLM provider (behind `IAiService`), Gmail access method (mailbox OAuth vs. service account), auth mechanism (Google SSO allow-list vs. email/password, with `User.Role` = Admin/Agent), and deployment target. The first deployed admin is the project owner (bootstrap), who can then create agent logins.
