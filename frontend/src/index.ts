import { serve } from "bun";
import index from "./index.html";

const server = serve({
  routes: {
    // Serve index.html for all unmatched routes.
    "/*": index,

    // Forward all /api/* requests to the backend API.
    "/api/*": req => {
      const url = new URL(req.url);
      return fetch(
        new Request("http://localhost:5155" + url.pathname + url.search, req),
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
