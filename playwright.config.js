import { defineConfig } from "@playwright/test";
import os from "node:os";
import path from "node:path";

const e2eDatabasePath = path.join(os.tmpdir(), `buza-e2e-${Date.now()}.db`);

export default defineConfig({
  testDir: "./e2e",
  timeout: 45_000,
  fullyParallel: false,
  reporter: [["list"]],
  use: {
    baseURL: "http://127.0.0.1:5179",
    viewport: { width: 1366, height: 900 },
    trace: "on-first-retry"
  },
  webServer: {
    command: "dotnet run --no-build --urls http://127.0.0.1:5179",
    url: "http://127.0.0.1:5179/api/products",
    reuseExistingServer: false,
    timeout: 120_000,
    env: {
      ...process.env,
      ASPNETCORE_ENVIRONMENT: "Testing",
      ConnectionStrings__DefaultConnection: `Data Source=${e2eDatabasePath}`
    }
  }
});
