# Helpdesk — Tech Stack

The MVP is split into a **React/TypeScript frontend** (tooled with Bun) and a
**.NET 10 controller-based Web API** backend organized as a clean, multi-project
solution using EF Core over PostgreSQL.

## At a glance

| Layer | Choice |
|---|---|
| Frontend | React + TypeScript, tooled with **Bun** |
| Backend API | **.NET 10**, controller-based Web API (not Minimal API) |
| ORM | **EF Core** |
| Database | **PostgreSQL** |
| Architecture | Clean, multi-project solution (API / Core / Infrastructure) |

---

## Frontend — React + TypeScript (Bun)

- **React + TypeScript** for the agent UI: ticket review queue (list with filter/sort)
  and the review screen (summary, editable draft, send).
- **Bun** as the package manager, dev server, and bundler/build tool — fast installs and
  a single toolchain.
- Talks to the backend over **REST/JSON**. API base URL is configured via environment
  variable so the frontend and API deploy independently.
- TypeScript types for API contracts (Ticket, Draft, Category) kept in sync with the
  backend DTOs.

## Backend — .NET 10 Web API (controller-based)

- **ASP.NET Core controllers** (attribute-routed), **not** Minimal API — explicit
  controller classes per resource (e.g. `TicketsController`, `AuthController`).
- Dependency injection wires repositories and services into controllers.
- Returns DTOs (not entities) across the API boundary.

## Persistence — EF Core + PostgreSQL

- **EF Core** as the ORM with the **Npgsql** provider for PostgreSQL.
- Code-first **migrations** checked into source control.
- `DbContext` and repository implementations live in the Infrastructure project.

---

## Solution structure (clean, multi-project)

```
Helpdesk.sln
├── src/
│   ├── Helpdesk.Api/              # ASP.NET Core Web API (controllers, DI, config)
│   ├── Helpdesk.Core/            # Entities + interfaces (no external dependencies)
│   └── Helpdesk.Infrastructure/  # EF Core DbContext, migrations, repositories,
│                                  # external integrations (Gmail, LLM)
└── frontend/                     # React + TypeScript app (Bun)
```

### Dependency direction (Clean Architecture)

```
Helpdesk.Api  ──▶  Helpdesk.Core  ◀──  Helpdesk.Infrastructure
```

- **Core** depends on nothing. It defines the domain and the contracts everyone else
  implements or consumes.
- **Infrastructure** depends on **Core** (implements its interfaces).
- **Api** depends on **Core** (and references **Infrastructure** only for DI wiring at
  startup / composition root).

### Project responsibilities

**Helpdesk.Core** — the domain, dependency-free.
- Entities: `Ticket`, `User` (admin/agent), `Reply`/`Draft`, category enum, ticket state.
- Interfaces: `ITicketRepository`, `IUserRepository`, plus service contracts such as
  `IEmailClient` (Gmail), `IAiService` (classify/summarize/draft), `IKnowledgeBase`.

**Helpdesk.Infrastructure** — implementations and I/O.
- `AppDbContext` (EF Core) + entity configurations + migrations.
- Repository implementations (`TicketRepository`, `UserRepository`, …).
- External integrations: Gmail client, LLM/AI service, the hardcoded knowledge base.

**Helpdesk.Api** — the HTTP surface and composition root.
- Controllers (`TicketsController`, `AuthController`, …).
- DTOs + mapping, request validation, auth/role enforcement (admin vs agent).
- `Program.cs`: DI registration, EF Core/Npgsql config, CORS for the Bun frontend,
  connection strings and secrets via configuration.

> Optional later: a `Helpdesk.Application` project for use-case/service orchestration if
> business logic outgrows Core. Not needed for the MVP.

---

## How the stack maps to the MVP core loop

1. **Ingest** — Infrastructure Gmail client pulls emails; a service creates `Ticket`
   entities via `ITicketRepository`.
2. **Classify + summarize** — `IAiService` (Infrastructure) sets category + summary.
3. **Draft** — `IAiService` drafts a reply using `IKnowledgeBase` (hardcoded).
4. **Review + send** — React queue calls `TicketsController`; agent edits the draft;
   send endpoint dispatches via the Gmail client and marks the ticket Sent.

## Still open (not stack decisions)
- LLM provider behind `IAiService`.
- Gmail access method (OAuth on the mailbox vs. service account).
- Auth mechanism for admin/agent logins.
- Deployment target / hosting for the API, Postgres, and frontend.
