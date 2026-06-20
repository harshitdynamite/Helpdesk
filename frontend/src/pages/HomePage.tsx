import { useAuth } from "../auth/AuthContext";

export function HomePage() {
  const { session } = useAuth();

  return (
    <div>
      <h1 className="text-3xl font-bold text-foreground">
        Welcome, {session?.displayName}
      </h1>
      <p className="mt-2 text-muted-foreground">
        The ticket review queue will live here.
      </p>
    </div>
  );
}
