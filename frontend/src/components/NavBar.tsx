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
    <nav className="flex items-center justify-between border-b border-gray-800 bg-gray-900 px-6 py-3.5">
      <span className="text-lg font-bold text-white">Helpdesk</span>
      <div className="flex items-center gap-4">
        <span className="text-gray-300">{session?.displayName}</span>
        <button
          type="button"
          className="rounded-lg bg-blue-600 px-4 py-1.5 font-bold text-white transition hover:-translate-y-px hover:bg-blue-700"
          onClick={handleSignOut}
        >
          Sign out
        </button>
      </div>
    </nav>
  );
}
