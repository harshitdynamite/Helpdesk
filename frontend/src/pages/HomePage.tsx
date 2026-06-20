import { useAuth } from "../auth/AuthContext";

export function HomePage() {
  const { session } = useAuth();

  return (
    <div className="home">
      <h1>Welcome, {session?.displayName}</h1>
      <p className="home-hint">
        The ticket review queue will live here.
      </p>
    </div>
  );
}
