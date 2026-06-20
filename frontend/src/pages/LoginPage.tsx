import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Navigate, useNavigate } from "react-router";
import { useAuth } from "../auth/AuthContext";
import { login as loginRequest } from "../api/client";

const loginSchema = z.object({
  email: z.email("Enter a valid email address"),
  password: z.string().min(1, "Password is required"),
});

type LoginForm = z.infer<typeof loginSchema>;

export function LoginPage() {
  const { isAuthenticated, login } = useAuth();
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<LoginForm>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: "", password: "" },
  });

  // Already signed in → no reason to show the login form.
  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  const onSubmit = async (values: LoginForm) => {
    try {
      const session = await loginRequest(values.email, values.password);
      login(session);
      navigate("/", { replace: true });
    } catch (err) {
      setError("root", {
        message: err instanceof Error ? err.message : "Something went wrong.",
      });
    }
  };

  return (
    <div className="flex min-h-screen w-full items-center justify-center bg-gray-50 p-8">
      <form
        className="flex w-full max-w-sm flex-col gap-4 rounded-2xl border border-gray-800 bg-gray-900 p-8 text-left text-gray-100 shadow-xl"
        onSubmit={handleSubmit(onSubmit)}
        noValidate
      >
        <h1 className="text-center text-3xl font-bold text-white">Helpdesk</h1>
        <p className="mb-2 text-center text-gray-400">Sign in to your account</p>

        <label className="flex flex-col gap-1.5 text-sm">
          <span className="font-semibold">Email</span>
          <input
            type="email"
            autoComplete="username"
            autoFocus
            aria-invalid={errors.email ? "true" : undefined}
            className="rounded-lg border border-gray-700 bg-gray-800 px-3 py-2.5 text-base text-white outline-none transition-colors focus:border-blue-500 aria-[invalid=true]:border-red-500"
            {...register("email")}
          />
          {errors.email && (
            <p className="text-sm text-red-400">{errors.email.message}</p>
          )}
        </label>

        <label className="flex flex-col gap-1.5 text-sm">
          <span className="font-semibold">Password</span>
          <input
            type="password"
            autoComplete="current-password"
            aria-invalid={errors.password ? "true" : undefined}
            className="rounded-lg border border-gray-700 bg-gray-800 px-3 py-2.5 text-base text-white outline-none transition-colors focus:border-blue-500 aria-[invalid=true]:border-red-500"
            {...register("password")}
          />
          {errors.password && (
            <p className="text-sm text-red-400">{errors.password.message}</p>
          )}
        </label>

        {errors.root && (
          <p className="text-sm text-red-400">{errors.root.message}</p>
        )}

        <button
          type="submit"
          className="rounded-lg bg-blue-600 px-5 py-2.5 font-bold text-white transition hover:-translate-y-px hover:bg-blue-700 disabled:translate-y-0 disabled:cursor-default disabled:opacity-60 disabled:hover:bg-blue-600"
          disabled={isSubmitting}
        >
          {isSubmitting ? "Signing in…" : "Sign in"}
        </button>
      </form>
    </div>
  );
}
