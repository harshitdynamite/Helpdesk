/**
 * Production build.
 *
 * The `bun build` CLI does not yet honor `[serve.static]` plugins from
 * bunfig.toml, so Tailwind would be left uncompiled. Building through the
 * `Bun.build` API lets us register `bun-plugin-tailwind` explicitly. The dev
 * server (`bun --hot src/index.ts`) picks up the same plugin via bunfig.toml.
 */
import tailwind from "bun-plugin-tailwind";

const result = await Bun.build({
  entrypoints: ["./src/index.html"],
  outdir: "./dist",
  target: "browser",
  sourcemap: "linked",
  minify: true,
  plugins: [tailwind],
  define: { "process.env.NODE_ENV": '"production"' },
  env: "BUN_PUBLIC_*",
});

if (!result.success) {
  for (const log of result.logs) console.error(log);
  process.exit(1);
}

for (const output of result.outputs) {
  console.log(output.path);
}
