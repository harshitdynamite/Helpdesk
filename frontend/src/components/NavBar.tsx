import { useNavigate } from "react-router";
import { useAuth } from "../auth/AuthContext";

export function NavBar() {
  const { session, logout } = useAuth();
  const navigate = useNavigate();

  const handleSignOut = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <nav className="nav-bar">
      <span className="nav-brand">Helpdesk</span>
      <div className="nav-user">
        <span className="nav-name">{session?.displayName}</span>
        <button type="button" className="nav-signout" onClick={handleSignOut}>
          Sign out
        </button>
      </div>
    </nav>
  );
}
