import { Link, useNavigate } from "react-router";
import { useAuth } from "../auth/AuthContext";
import { Button } from "@/components/ui/button";
import { ModeToggle } from "./ModeToggle";

export function NavBar() {
  const { session, logout } = useAuth();
  const navigate = useNavigate();

  const handleSignOut = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <nav className="flex items-center justify-between border-b bg-card px-6 py-3">
      <div className="flex items-center gap-6">
        <Link to="/" className="text-lg font-bold">
          Helpdesk
        </Link>
        {session?.role === "Admin" && (
          <Link
            to="/users"
            className="text-sm text-muted-foreground hover:text-foreground"
          >
            Users
          </Link>
        )}
      </div>
      <div className="flex items-center gap-4">
        <span className="text-sm text-muted-foreground">
          {session?.displayName}
        </span>
        <ModeToggle />
        <Button type="button" variant="outline" size="sm" onClick={handleSignOut}>
          Sign out
        </Button>
      </div>
    </nav>
  );
}
