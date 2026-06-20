import { serve } from "bun";
import index from "./index.html";

// Port and backend target default to the dev values but can be overridden (e.g. the E2E
// run uses a dedicated port pair so it can coexist with the dev servers).
const port = Number(process.env.PORT) || 3000;
const apiTarget = process.env.API_PROXY_TARGET ?? "http://localhost:5155";

const server = serve({
  port,
  routes: {
    // Serve index.html for all unmatched routes.
    "/*": index,

    // Forward all /api/* requests to the backend API.
    "/api/*": req => {
      const url = new URL(req.url);
      return fetch(
        new Request(apiTarget + url.pathname + url.search, req),
      );
    },
  },

  development: process.env.NODE_ENV !== "production" && {
    // Enable browser hot reloading in development
    hmr: true,

    // Echo console logs from the browser to the server
    console: true,
  },
});

console.log(`🚀 Server running at ${server.url}`);
