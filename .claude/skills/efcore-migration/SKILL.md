---
name: efcore-migration
description: >-
  Use whenever a change to the EF Core model requires a new database migration in this
  repo (Helpdesk.Core entities + Helpdesk.Infrastructure AppDbContext/configurations over
  PostgreSQL). Trigger after ANY edit that alters the persisted schema, including: adding,
  removing, or renaming an entity in Helpdesk.Core/Entities; adding, removing, renaming, or
  retyping a property; making a property required/optional or changing its max length;
  adding, changing, or removing a relationship, foreign key, or navigation property; adding
  or removing an index or unique constraint; editing an IEntityTypeConfiguration in
  Helpdesk.Infrastructure/Data/Configurations; adding, changing, or removing HasData seed
  rows; adding a DbSet or otherwise changing the AppDbContext model; or changing how an enum
  maps to a column. Also use when the user says "add a migration", "update the database",
  "create the schema", when migrations are pending or out of sync, or when a model change is
  about to be committed without a matching migration. Guides creating, reviewing, applying,
  verifying, and committing the migration with the project's exact commands.
---

# EF Core migrations (Helpdesk)

This project keeps the schema in C# and uses EF Core migrations over PostgreSQL. When the
model changes, the migration must change with it — in the **same commit** as the model edit.

## Project layout you need to know

- **Entities:** `src/Helpdesk.Core/Entities/` (`Ticket`, `User`, `Draft`).
- **Configurations:** `src/Helpdesk.Infrastructure/Data/Configurations/` (`IEntityTypeConfiguration` classes — column types, lengths, indexes, relationships, `HasData`).
- **Context:** `src/Helpdesk.Infrastructure/Data/AppDbContext.cs` (DbSets, auto timestamps).
- **Migrations + snapshot:** `src/Helpdesk.Infrastructure/Data/Migrations/`.
- **Design-time factory:** `AppDbContextFactory` builds the context for the CLI and reads the
  connection string from the **`HELPDESK_DB`** environment variable (falling back to a local
  default). The migration tools do **not** read `appsettings`.
- EF runs against the Infrastructure project as both `--project` and `--startup-project`
  (it's a class library with the design-time factory; there is no `.sln`).

## Does this change actually need a migration?

**Needs a migration** — anything that changes the database shape or migration-seeded data:
new/removed/renamed entity, property add/remove/rename/retype, required vs optional, max
length, relationship/FK/navigation changes, index or unique-constraint changes, enum-to-column
mapping changes, a new `DbSet`, or edits to a `HasData(...)` call.

**Does NOT need a migration:**
- DTOs, controllers, services, frontend, or any runtime-only logic.
- Editing the **sample tickets/drafts** in `SeedData.Tickets()` / `SeedData.Drafts()`. Those
  are inserted at runtime by `DevelopmentDataSeeder` (Development only), **not** via `HasData`.
- Note the asymmetry: the **owner user** *is* seeded through `HasData` in `UserConfiguration`,
  so changing the owner row **does** require a migration; changing sample tickets/drafts does not.

If unsure, run step 1 — if the generated migration's `Up()` is empty, delete it (step 5b).

## Workflow

### 1. Build first
A migration is scaffolded by diffing the compiled model against the snapshot, so the code must
compile.
```powershell
dotnet build src/Helpdesk.Api
```

### 2. Create the migration
Pick a PascalCase name describing the change (e.g. `AddTicketPriority`, `MakeSummaryRequired`).
```powershell
dotnet ef migrations add <Name> `
  --project src/Helpdesk.Infrastructure `
  --startup-project src/Helpdesk.Infrastructure `
  --output-dir Data/Migrations
```
This only generates files (`<timestamp>_<Name>.cs`, `.Designer.cs`, and an updated
`AppDbContextModelSnapshot.cs`). It does not touch any database.

### 3. Review the generated migration — always
Open the new `Up()`/`Down()` and confirm it matches your intent. Watch for:
- **"An operation was scaffolded that may result in the loss of data"** — EF prints this for
  `DropColumn`, `DropTable`, `DeleteData`, or type narrowing. Make sure the loss is intended.
- A **rename** scaffolded as drop + add (loses data). If you meant to rename, switch it to
  `migrationBuilder.RenameColumn(...)` / `RenameTable(...)` by hand.
- Required new columns on a non-empty table need a default, or the apply will fail.

### 4. Apply to the database
The CLI reads `HELPDESK_DB`. Use the same connection string as
`src/Helpdesk.Api/appsettings.Development.local.json` (gitignored). Do not hardcode the
password anywhere committed.
```powershell
$env:HELPDESK_DB = 'Host=localhost;Port=5432;Database=helpdesk;Username=postgres;Password=<your-dev-password>'
dotnet ef database update `
  --project src/Helpdesk.Infrastructure `
  --startup-project src/Helpdesk.Infrastructure
```
EF applies every migration not yet recorded in the `__EFMigrationsHistory` table.
> The app does **not** auto-migrate at startup — applying is always this manual step. On a
> fresh database: `database update` first (creates schema + owner), then `dotnet run` (the
> Development seeder adds sample tickets/drafts).

### 5. Verify
Confirm the schema/data landed (psql is at `C:\Program Files\PostgreSQL\18\bin\psql.exe`):
```powershell
$env:PGPASSWORD = '<your-dev-password>'
& "C:\Program Files\PostgreSQL\18\bin\psql.exe" -w -U postgres -h localhost -d helpdesk `
  -c '\d+ tickets' -c 'select "MigrationId" from "__EFMigrationsHistory"'
```

**5b. Undo a migration you haven't shipped yet.** If the diff is wrong or empty, remove it and
re-create (this also reverts the snapshot):
```powershell
dotnet ef migrations remove --project src/Helpdesk.Infrastructure --startup-project src/Helpdesk.Infrastructure
```
If it was already applied to your dev DB, first roll the database back to the previous
migration with `dotnet ef database update <PreviousMigrationName>`, then `migrations remove`.

### 6. Commit migration + model change together
Stage the model edit **and** all three migration artifacts (`*.cs`, `*.Designer.cs`,
`AppDbContextModelSnapshot.cs`) in one commit, so the schema is never out of sync with the
code in history. The gitignored `appsettings.Development.local.json` must never be committed.

## Common pitfalls
- Editing an entity but forgetting to add a migration → schema drifts from code. After any
  model edit, run step 2; an empty `Up()` confirms nothing was needed.
- Committing the model change without the snapshot/migration files.
- Expecting the running app to apply migrations — it won't; run `database update`.
- Putting the connection string / password in a committed file instead of `HELPDESK_DB` or
  `appsettings.Development.local.json`.
