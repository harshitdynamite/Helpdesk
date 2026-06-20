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

// Dedicated E2E ports (distinct from dev's 5155/3000) so the E2E stack can run alongside
// the dev servers without colliding. Overridable via env.
const apiPort = process.env.E2E_API_PORT ?? "5200";
const webPort = process.env.E2E_WEB_PORT ?? "3100";
const apiUrl = `http://localhost:${apiPort}`;
const webUrl = `http://localhost:${webPort}`;

// Fixed, non-secret test values — safe to commit. They only ever apply to the throwaway
// test database under the Testing environment, never to dev or prod.
const apiEnv: Record<string, string> = {
  ASPNETCORE_ENVIRONMENT: "Testing",
  ASPNETCORE_URLS: apiUrl,
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
    baseURL: webUrl,
    trace: "on-first-retry",
  },
  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],
  // Full-stack orchestration: the API migrates/creates the test DB and boots against it on
  // a dedicated port, then the frontend dev server (proxying /api/* -> the test API) comes
  // up on its own port. These ports are distinct from dev's 5155/3000, so E2E coexists with
  // a running dev stack — and reuse is safe because nothing else binds the E2E ports.
  webServer: [
    {
      command: "bun run e2e/start-api.ts",
      url: `${apiUrl}/api/health`,
      env: apiEnv,
      reuseExistingServer: !isCI,
      timeout: 120_000,
      stdout: "pipe",
      stderr: "pipe",
    },
    {
      command: "bun dev",
      url: webUrl,
      env: { PORT: webPort, API_PROXY_TARGET: apiUrl },
      reuseExistingServer: !isCI,
      timeout: 60_000,
    },
  ],
});
