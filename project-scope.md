# Helpdesk — MVP Scope

## Problem
We receive several support emails daily. Human agents manually read, classify, and
respond to each ticket — which is slow and leads to impersonal, canned responses.

## Solution (MVP)
An AI-assisted ticket system that ingests support emails, classifies and summarizes
them, and **drafts** a reply using a simple knowledge base. Every reply is reviewed,
edited, and sent by a human agent. The MVP proves the core loop end-to-end; everything
else is deferred.

## Core loop (the entire MVP)
1. **Ingest** — pull support emails from Gmail and create a ticket for each.
2. **Classify + summarize** — AI assigns a category and writes a short summary per ticket.
3. **Draft** — AI drafts a suggested reply using a **simple hardcoded knowledge base**.
4. **Review + send** — an agent sees the queue, reviews the draft, edits it, and sends
   the reply back to the student.

## In scope
- Gmail ingestion → ticket creation (with thread association).
- AI classification into a small fixed taxonomy.
- AI summary per ticket.
- AI-drafted reply grounded in a hardcoded KB (a few in-code articles/snippets).
- Agent **review queue**: list of tickets with their drafted replies.
- Review screen: edit the draft and send via Gmail in the original thread.
- **Auth:** deploy with the project owner as bootstrap **admin**; admin can create an
  **agent** role/login for other people. Minimal — just enough to gate access by role.

## Explicitly out of scope (deferred)
- Knowledge base ingestion (KB is hardcoded in the MVP).
- User management UI (admin creates agents, but no full management screens).
- Dashboard / analytics.
- Audit trail.
- Auto-acknowledgement emails.
- Complex ticket states/lifecycle (keep states minimal — e.g. Needs review → Sent).
- Live chat, student portal, multi-language, PII redaction, SLA/escalation,
  auto-sending without a human.

## Taxonomy (starting set)
Account/Login · Billing · Academic · Technical · Other.

## Minimal ticket states
Needs review → Sent. (No reopen logic, no awaiting-reply tracking in the MVP.)

## Locked context (from scoping)
- Email source: Google Workspace / Gmail (`support@`).
- Reply mode: human approves first (no auto-send).
- PII: OK to send ticket text to the LLM; no redaction.
- Scale: low (<50/day, small team) — simple shared queue.

## To decide before building
- Tech stack (language + framework + storage).
- LLM provider.
- Gmail access method (OAuth on the mailbox vs. service account; or mock for now).
- Login mechanism for admin/agent (Google SSO allow-list vs. email/password).
- Deployment target.
