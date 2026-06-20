import type { ReactNode } from "react";
import { Navigate } from "react-router";
import { useAuth } from "../auth/AuthContext";

/**
 * Renders children only when the signed-in user has the required role.
 * Unauthenticated users go to /login; authenticated-but-wrong-role users are
 * sent back to the home page (they have no business on the guarded route).
 */
export function RequireRole({
  role,
  children,
}: {
  role: string;
  children: ReactNode;
}) {
  const { session } = useAuth();

  if (session === null) {
    return <Navigate to="/login" replace />;
  }
  if (session.role !== role) {
    return <Navigate to="/" replace />;
  }
  return <>{children}</>;
}
