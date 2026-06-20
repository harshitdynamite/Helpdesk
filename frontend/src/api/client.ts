import type { Session } from "../auth/AuthContext";

/** Shape returned by POST /api/auth/login (camelCase from the .NET API). */
type LoginResponse = Session;

/**
 * Log in with email + password. Requests go to a relative `/api/*` URL so they
 * flow through the Bun dev server proxy to the backend (same origin → no CORS).
 * Throws an Error with a user-friendly message on failure.
 */
export async function login(
  email: string,
  password: string,
): Promise<LoginResponse> {
  let res: Response;
  try {
    res = await fetch("/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    });
  } catch {
    throw new Error("Could not reach the server. Is the backend running?");
  }

  if (res.status === 401) {
    throw new Error("Invalid email or password.");
  }
  if (!res.ok) {
    throw new Error(`Login failed (${res.status}). Please try again.`);
  }

  return (await res.json()) as LoginResponse;
}

/**
 * Authorization header for authenticated requests to the API. Reused by future
 * calls once we start hitting protected endpoints (e.g. the ticket queue).
 */
export function authHeader(token: string): Record<string, string> {
  return { Authorization: `Bearer ${token}` };
}
