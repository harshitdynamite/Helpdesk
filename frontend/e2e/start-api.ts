/**
 * Boots the Helpdesk API for E2E runs against the dedicated test database.
 *
 * Guarantees migrate-before-serve: the schema is applied (and the database created if
 * absent) BEFORE `dotnet run` starts, so the API's startup seeders never hit a missing
 * schema. This runs as a Playwright `webServer` command, inheriting the Testing-env vars
 * (ConnectionStrings__Default, Jwt__SigningKey, Bootstrap__*) from playwright.config.ts.
 */
import { $ } from "bun";

const connectionString = process.env.ConnectionStrings__Default;
if (!connectionString) {
  console.error(
    "ConnectionStrings__Default is not set. Run via `bun run e2e` so " +
      "playwright.config.ts injects the Testing environment.",
  );
  process.exit(1);
}

// 1) Create the test DB if needed and apply all migrations. `database update` is idempotent.
//    This repo drives EF design-time tooling through Helpdesk.Infrastructure's
//    AppDbContextFactory, which reads the connection from the HELPDESK_DB env var (the Api
//    project intentionally does not reference EF Core Design).
console.log("[e2e] Applying migrations to the test database...");
await $`dotnet ef database update \
  --project ../src/Helpdesk.Infrastructure \
  --startup-project ../src/Helpdesk.Infrastructure`.env({
  ...process.env,
  HELPDESK_DB: connectionString,
});

// 2) Run the API. --no-launch-profile so launchSettings' Development profile is bypassed and
//    the Testing env vars from the parent process take effect. stdio is inherited so the
//    Playwright web server can read readiness from the API's own logs / health endpoint.
console.log("[e2e] Starting the API under the Testing environment...");
const api = Bun.spawn({
  cmd: ["dotnet", "run", "--project", "../src/Helpdesk.Api", "--no-launch-profile"],
  stdout: "inherit",
  stderr: "inherit",
  env: process.env,
});

// Forward termination so Playwright tearing down the web server stops dotnet too.
const shutdown = () => {
  api.kill();
};
process.on("SIGINT", shutdown);
process.on("SIGTERM", shutdown);

const code = await api.exited;
process.exit(code);
