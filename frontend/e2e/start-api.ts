/**
 * Boots the Helpdesk API for E2E runs against the dedicated test database.
 *
 * Guarantees migrate-before-serve: the schema is applied (and the database created if
 * absent) BEFORE `dotnet run` starts, so the API's startup seeders never hit a missing
 * schema. This runs as a Playwright `webServer` command, inheriting the Testing-env vars
 * (ConnectionStrings__Default, Jwt__SigningKey, Bootstrap__*) from playwright.config.ts.
 */
import { existsSync } from "node:fs";
import { $ } from "bun";

const connectionString = process.env.ConnectionStrings__Default;
if (!connectionString) {
  console.error(
    "ConnectionStrings__Default is not set. Run via `bun run e2e` so " +
      "playwright.config.ts injects the Testing environment.",
  );
  process.exit(1);
}

// We deliberately run the already-built binaries (--no-build / --no-restore below). The E2E
// API shares the projects' default bin/ with a running dev `dotnet run`; rebuilding here would
// fail with file-lock errors (MSB3027) because the dev process holds those DLLs open. Running
// the existing build lets the E2E stack coexist with the dev servers. Build only when no
// artifacts exist yet (clean checkout / dev not running), where locking is not a concern.
const apiDll = "../src/Helpdesk.Api/bin/Debug/net10.0/Helpdesk.Api.dll";
if (!existsSync(apiDll)) {
  console.log("[e2e] No build found — building the API once...");
  await $`dotnet build ../src/Helpdesk.Api`;
}

// 1) Create the test DB if needed and apply all migrations. `database update` is idempotent.
//    This repo drives EF design-time tooling through Helpdesk.Infrastructure's
//    AppDbContextFactory, which reads the connection from the HELPDESK_DB env var (the Api
//    project intentionally does not reference EF Core Design).
console.log("[e2e] Applying migrations to the test database...");
await $`dotnet ef database update \
  --project ../src/Helpdesk.Infrastructure \
  --startup-project ../src/Helpdesk.Infrastructure \
  --no-build`.env({
  ...process.env,
  HELPDESK_DB: connectionString,
});

// 2) Run the API. --no-launch-profile so launchSettings' Development profile is bypassed and
//    the Testing env vars from the parent process take effect. --no-build avoids contending
//    with a running dev API over the shared bin/ output. stdio is inherited so the Playwright
//    web server can read readiness from the API's own logs / health endpoint.
console.log("[e2e] Starting the API under the Testing environment...");
const api = Bun.spawn({
  cmd: [
    "dotnet",
    "run",
    "--project",
    "../src/Helpdesk.Api",
    "--no-launch-profile",
    "--no-build",
  ],
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
