# Helpdesk — AI-Powered Support Ticket System

> Built live on YouTube using Claude Code · Full Stack .NET + React series

This is the companion repository for the YouTube series **"Full Stack .NET + React with Claude Code"** — where we build a production-grade, AI-powered helpdesk from scratch, one phase at a time, using Claude Code as our development partner.

---

## 🎬 Watch the Series on my Youtube channel ([https://youtube.com/playlist?list=PLf_tLWRz3DTOo4lC8UKmgXAEsx0KahHdr&si=Xa6OwflNg_gtlF0X])

| Part | Title | Status |
|------|-------|--------|
| Part 1 | The Problem & The Plan | ✅ Published |
| Part 2 | Domain Entities, EF Core & PostgreSQL | 🔜 Coming soon |
| Part 3 | REST API Foundation & Swagger | 🔜 Coming soon |
| Part 4 | Authentication with Auth0 | 🔜 Coming soon |
| Part 5 | Gmail Ingestion & Ticket Creation | 🔜 Coming soon |
| Part 6 | AI Classification, Summary & Reply Drafting | 🔜 Coming soon |
| Part 7 | Agent Review Queue & Send Reply | 🔜 Coming soon |
| Part 8 | Deployment & Hardening | 🔜 Coming soon |

---

## 🧠 What We're Building

A fully functional AI-powered helpdesk that:

- Ingests support emails from **Gmail** and creates tickets automatically
- Uses **Claude AI** to classify, summarize, and draft replies
- Lets human agents **review, edit, and approve** every reply before it's sent
- Supports **role-based access** — Admin creates and manages Agent accounts
- Is built as a **production-grade, deployable system** — not a tutorial toy

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | React 19 + TypeScript via Bun |
| Backend | .NET 10 Web API (Controller-based) |
| ORM | Entity Framework Core |
| Database | PostgreSQL |
| Authentication | Auth0 |
| Email | Gmail API |
| AI | Claude API (Anthropic) |
| Architecture | Clean Architecture (Core / Infrastructure / Api) |

---

## 📁 Project Structure

```
Helpdesk/
├── frontend/                  # React + TypeScript (Bun)
│   └── src/
├── src/
│   ├── Helpdesk.Api/          # Controllers, DTOs, Program.cs
│   ├── Helpdesk.Core/         # Entities, Interfaces (no dependencies)
│   └── Helpdesk.Infrastructure/ # EF Core, Repositories, Services
├── CLAUDE.md                  # Claude Code project memory
├── implementation-plan.md     # Phased build plan
├── project-scope.md           # MVP scope and decisions
└── tech-stack.md              # Tech stack rationale
```

### Dependency Direction (Clean Architecture)
```
Helpdesk.Api ──→ Helpdesk.Core ←── Helpdesk.Infrastructure
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Bun](https://bun.sh)
- [PostgreSQL 18](https://www.postgresql.org/download/)
- [Claude Code](https://claude.ai/code)

### Backend

```bash
# From the repo root
dotnet restore
dotnet build
dotnet run --project src/Helpdesk.Api
```

### Frontend

```bash
cd frontend
bun install
bun dev
```

### Database

```bash
# Set your connection string in user secrets
dotnet user-secrets set "ConnectionStrings:Helpdesk" "Host=localhost;Port=5432;Database=helpdesk;Username=postgres;Password=YOUR_PASSWORD" --project src/Helpdesk.Api

# Apply migrations
dotnet ef database update --project src/Helpdesk.Infrastructure --startup-project src/Helpdesk.Api
```

---

## 📦 Current Phase

**✅ Phase 0 — Tooling & Solution Skeleton**
- .NET 10 solution with 3 projects (Api, Core, Infrastructure)
- React + TypeScript frontend via Bun
- Health check endpoint (`GET /api/health`)
- Frontend proxies API calls to backend
- CLAUDE.md project memory initialised
- Git repository with branch protection on `main`

**🔜 Next — Phase 1: Domain & Persistence**

---

## 🤝 How to Follow Along

1. **Watch** the YouTube series here -> [https://youtube.com/playlist?list=PLf_tLWRz3DTOo4lC8UKmgXAEsx0KahHdr&si=Xa6OwflNg_gtlF0X]
2. **Star** this repo to follow progress
3. **Clone** the repo and build alongside the videos
4. Each phase ends in a git commit — checkout the tag to start from any phase

> ⚠️ This repo is read-only for contributors. The `main` branch is protected — PRs require approval before merging.

---

## 📺 About the Series

This series is about showing what's actually possible when you combine a proper software engineering mindset with AI-powered development tools. Every architectural decision is explained. Every mistake is shown. No hand-waving.

By the end of the series you'll have seen how to build a complete, deployable, production-grade full stack application using:

- Clean Architecture in .NET
- EF Core migrations done right
- Role-based authentication
- AI pipelines with the Claude API
- Gmail integration
- Automated testing

---

## 📬 Connect

- **YouTube**: [https://youtube.com/@harshitdynamite?si=edQ2-fm-LbG0w9_d]
- **GitHub**: [@harshitdynamite](https://github.com/harshitdynamite)

---

> Co-built with [Claude Code](https://claude.ai/code) by Anthropic
