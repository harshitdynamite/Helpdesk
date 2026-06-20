import { createContext, useContext, useEffect, useState } from "react";

type Theme = "dark" | "light" | "system";

type ThemeProviderProps = {
  children: React.ReactNode;
  defaultTheme?: Theme;
  storageKey?: string;
};

type ThemeProviderState = {
  theme: Theme;
  setTheme: (theme: Theme) => void;
  /** The theme actually applied to the document, resolving "system". */
  resolvedTheme: "dark" | "light";
};

const ThemeProviderContext = createContext<ThemeProviderState | undefined>(
  undefined,
);

function systemTheme(): "dark" | "light" {
  return window.matchMedia("(prefers-color-scheme: dark)").matches
    ? "dark"
    : "light";
}

export function ThemeProvider({
  children,
  defaultTheme = "system",
  storageKey = "helpdesk-theme",
}: ThemeProviderProps) {
  const [theme, setThemeState] = useState<Theme>(
    () => (localStorage.getItem(storageKey) as Theme | null) ?? defaultTheme,
  );
  const [resolvedTheme, setResolvedTheme] = useState<"dark" | "light">(() =>
    theme === "system" ? systemTheme() : theme,
  );

  useEffect(() => {
    const root = window.document.documentElement;
    const apply = (value: "dark" | "light") => {
      root.classList.remove("light", "dark");
      root.classList.add(value);
      setResolvedTheme(value);
    };

    if (theme !== "system") {
      apply(theme);
      return;
    }

    // Follow the OS preference and keep tracking it while on "system".
    const media = window.matchMedia("(prefers-color-scheme: dark)");
    apply(media.matches ? "dark" : "light");
    const onChange = (e: MediaQueryListEvent) =>
      apply(e.matches ? "dark" : "light");
    media.addEventListener("change", onChange);
    return () => media.removeEventListener("change", onChange);
  }, [theme]);

  const setTheme = (next: Theme) => {
    localStorage.setItem(storageKey, next);
    setThemeState(next);
  };

  return (
    <ThemeProviderContext.Provider value={{ theme, setTheme, resolvedTheme }}>
      {children}
    </ThemeProviderContext.Provider>
  );
}

export function useTheme() {
  const context = useContext(ThemeProviderContext);
  if (context === undefined) {
    throw new Error("useTheme must be used within a ThemeProvider");
  }
  return context;
}
