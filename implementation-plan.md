# Helpdesk MVP — Implementation Plan

Tasks are grouped into phases that build on each other. Each phase ends in something
runnable/verifiable. External integrations (Gmail, LLM) are built behind interfaces and
**mocked first**, so the full loop works before real credentials exist.

Legend: `[ ]` todo. Phases are roughly sequential; ⚠️ marks a blocked-on-a-decision item.

---

## Phase 0 — Tooling & solution skeleton
Goal: empty but buildable solution + frontend, committed.

- [ ] Verify toolchain installed: .NET 10 SDK, Bun, PostgreSQL (or Docker for Postgres).
- [ ] Create `Helpdesk.sln` with `src/` projects: `Helpdesk.Api`, `Helpdesk.Core`,
      `Helpdesk.Infrastructure`.
- [ ] Set project references: Api → Core, Api → Infrastructure, Infrastructure → Core.
- [ ] Scaffold `frontend/` React + TypeScript app via Bun; confirm dev server runs.
- [ ] Add `.gitignore` (bin/obj, node_modules, .env, appsettings.*.local).
- [ ] Confirm `dotnet build` and `bun run dev` both succeed. Commit.

## Phase 1 — Domain & persistence
Goal: entities, repository contracts, EF Core wired to Postgres, first migration applied.

- [ ] **Core:** entities — `Ticket`, `User`, `Reply`/`Draft`; enums `Category`,
      `TicketState` (NeedsReview → Sent).
- [ ] **Core:** repository interfaces — `ITicketRepository`, `IUserRepository`.
- [ ] **Infrastructure:** `AppDbContext` + entity configurations.
- [ ] **Infrastructure:** Npgsql provider + connection string via configuration.
- [ ] **Infrastructure:** repository implementations.
- [ ] Create + apply initial EF Core migration; verify tables in Postgres.
- [ ] Seed a couple of sample tickets for local dev.

## Phase 2 — API foundation
Goal: ticket data is reachable over HTTP.

- [ ] DI/composition root in `Program.cs` (DbContext, repositories).
- [ ] DTOs + mapping (entities never cross the API boundary).
- [ ] `TicketsController`: list (filter by state/category, sort), get by id.
- [ ] CORS for the Bun frontend; health-check endpoint.
- [ ] OpenAPI/Swagger for manual testing. Verify list/detail return seeded data.

## Phase 3 — Auth & roles
Goal: admin can log in; admin can create agent logins; endpoints gated by role.
⚠️ Depends on the login-mechanism decision (Google SSO allow-list vs. email/password).

- [ ] Choose + wire auth scheme; `User` carries `Role` (Admin/Agent).
- [ ] Bootstrap the owner as the first Admin (seed/config).
- [ ] `AuthController`: login + (admin-only) create-agent endpoint.
- [ ] Role-based authorization attributes on protected endpoints.
- [ ] Minimal login UI in the frontend; store/attach the token.

## Phase 4 — Gmail ingestion
Goal: real (or mocked) support emails become tickets.

- [ ] **Core:** `IEmailClient` interface (fetch new messages, send reply).
- [ ] **Infrastructure:** `MockEmailClient` (reads seeded fixtures) to unblock the loop.
- [ ] Ingestion service: map message → `Ticket` (subject, body, sender, threadId, state
      = NeedsReview); dedupe by message id.
- [ ] Trigger endpoint/scheduled job to run ingestion.
- [ ] ⚠️ Real Gmail client via `googleapis`/Google SDK — depends on Gmail access decision
      (OAuth on mailbox vs. service account). Swap behind `IEmailClient`.

## Phase 5 — AI classify, summarize, draft
Goal: each ticket gets a category, summary, and a drafted reply.
⚠️ Depends on the LLM provider decision.

- [ ] **Core:** `IAiService` (classify, summarize, draft) and `IKnowledgeBase`.
- [ ] **Infrastructure:** hardcoded `KnowledgeBase` (a few in-code articles/snippets).
- [ ] **Infrastructure:** `MockAiService` (deterministic stub) to build/test the flow.
- [ ] **Infrastructure:** real LLM implementation behind `IAiService` once provider chosen.
- [ ] Wire into ingestion (or a follow-up step): on new ticket, set category + summary +
      draft reply, persist `Draft`.
- [ ] Expose category/summary/draft in the ticket DTOs.

## Phase 6 — Agent review queue (frontend)
Goal: the agent-facing core experience.

- [ ] Queue list: tickets needing review, with filter (category/state) and sort.
- [ ] Review screen: show original email, AI summary, category; editable draft textarea.
- [ ] Save edits to the draft (PATCH endpoint).
- [ ] Loading/empty/error states; basic styling.

## Phase 7 — Send reply (close the loop)
Goal: approved drafts go back to the student via Gmail.

- [ ] `TicketsController` send endpoint: validate, dispatch via `IEmailClient`, mark
      ticket `Sent`, persist final reply text.
- [ ] "Send" action in the review screen; optimistic UI + confirmation.
- [ ] End-to-end test with mocks: ingest → classify/summarize/draft → edit → send.

## Phase 8 — Hardening & deployment
Goal: deployable MVP.
⚠️ Depends on the deployment-target decision.

- [ ] Centralized error handling + structured logging in the API.
- [ ] Config/secrets management (connection string, API keys, OAuth tokens).
- [ ] Swap in real Gmail + real LLM; run the loop against a test mailbox.
- [ ] Dockerfile(s) / deploy config for API, Postgres, and frontend.
- [ ] README: setup, env vars, run instructions.

---

## Suggested build order & decision gates
- **Phases 0 → 2** need no external decisions — start immediately.
- **Phase 3** needs the **login-mechanism** decision.
- **Phase 4 (real Gmail)** needs the **Gmail access** decision — but mock first.
- **Phase 5 (real LLM)** needs the **LLM provider** decision — but mock first.
- **Phase 8** needs the **deployment target**.

Because Gmail and AI are mocked behind interfaces early, the entire core loop is
demonstrable end-to-end after Phase 7 even if no external credentials are wired yet.
