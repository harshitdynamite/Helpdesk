import { BrowserRouter, Navigate, Route, Routes } from "react-router";
import { ThemeProvider } from "./components/theme-provider";
import { AuthProvider } from "./auth/AuthContext";
import { RequireAuth } from "./components/RequireAuth";
import { RequireRole } from "./components/RequireRole";
import { NavBar } from "./components/NavBar";
import { LoginPage } from "./pages/LoginPage";
import { HomePage } from "./pages/HomePage";
import { UsersPage } from "./pages/UsersPage";
import "./index.css";

export function App() {
  return (
    <ThemeProvider defaultTheme="system">
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route
              path="/"
              element={
                <RequireAuth>
                  <div className="flex min-h-screen flex-col bg-muted">
                    <NavBar />
                    <main className="flex-1 p-8 text-center">
                      <HomePage />
                    </main>
                  </div>
                </RequireAuth>
              }
            />
            <Route
              path="/users"
              element={
                <RequireRole role="Admin">
                  <div className="flex min-h-screen flex-col bg-muted">
                    <NavBar />
                    <main className="flex-1 p-8 text-center">
                      <UsersPage />
                    </main>
                  </div>
                </RequireRole>
              }
            />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;
