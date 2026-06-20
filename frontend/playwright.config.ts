import { defineConfig, devices } from "@playwright/test";
import { config as loadEnv } from "dotenv";

// Load E2E-only secrets (DB password, etc.) from a gitignored frontend/.env.e2e.
// Copy frontend/.env.e2e.example to get started.
loadEnv({ path: ".env.e2e" });

// Assemble the connection string for the dedicated test database. Defaults target a
// local Postgres; override any piece via frontend/.env.e2e or real env vars.
const dbHost = process.env.E2E_DB_HOST ?? "localhost";
const dbPort = process.env.E2E_DB_PORT ?? "5432";
const dbName = process.env.E2E_DB_NAME ?? "helpdesk_e2e";
const dbUser = process.env.E2E_DB_USER ?? "postgres";
const dbPassword = process.env.E2E_DB_PASSWORD ?? "postgres";

const testConnectionString =
  `Host=${dbHost};Port=${dbPort};Database=${dbName};Username=${dbUser};Password=${dbPassword}`;

// Fixed, non-secret test values — safe to commit. They only ever apply to the throwaway
// test database under the Testing environment, never to dev or prod.
const apiEnv: Record<string, string> = {
  ASPNETCORE_ENVIRONMENT: "Testing",
  ASPNETCORE_URLS: "http://localhost:5155",
  ConnectionStrings__Default: testConnectionString,
  Jwt__SigningKey: "e2e-test-signing-key-do-not-use-in-prod-0123456789",
  Bootstrap__AdminEmail: "admin@e2e.test",
  Bootstrap__AdminPassword: "Admin!2345",
  Bootstrap__AdminDisplayName: "E2E Admin",
};

const isCI = !!process.env.CI;

export default defineConfig({
  testDir: "./e2e",
  fullyParallel: true,
  forbidOnly: isCI,
  retries: isCI ? 2 : 0,
  reporter: "html",
  use: {
    baseURL: "http://localhost:3000",
    trace: "on-first-retry",
  },
  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],
  // Full-stack orchestration: the API migrates/creates the test DB and boots against it,
  // then the frontend dev server (which proxies /api/* -> :5155) comes up.
  //
  // reuseExistingServer is intentionally false: a dev API on :5155 points at the *dev*
  // database, so reusing it would run tests against unseeded/dev data (no Testing-env
  // bootstrap admin). Forcing a fresh boot guarantees start-api.ts migrates+seeds
  // helpdesk_e2e every run, and that the frontend serves a current bundle. The trade-off
  // is that any process already holding :5155 / :3000 must be stopped first.
  webServer: [
    {
      command: "bun run e2e/start-api.ts",
      url: "http://localhost:5155/api/health",
      env: apiEnv,
      reuseExistingServer: false,
      timeout: 120_000,
      stdout: "pipe",
      stderr: "pipe",
    },
    {
      command: "bun dev",
      url: "http://localhost:3000",
      reuseExistingServer: false,
      timeout: 60_000,
    },
  ],
});
