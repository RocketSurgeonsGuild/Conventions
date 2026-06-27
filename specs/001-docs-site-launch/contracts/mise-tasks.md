# Contract: Mise Task Definitions

**Feature**: `specs/001-docs-site-launch`
**Date**: 2026-06-27

These are the canonical task definitions for the Conventions docs workflow. They replace the existing VitePress tasks in `.config/mise.toml`.

---

## Tasks

### `docs` — Start dev server

```toml
docs = { run = "cd docs && npm run dev", description = "Start Astro/Starlight dev server" }
```

**Replaces**: `docs = { run = "vitepress dev docs" }`
**Output**: Dev server at `http://localhost:4321`

---

### `docs:preview` — Preview production build

```toml
"docs:preview" = { run = "cd docs && npm run preview", description = "Preview built Starlight site" }
```

**Replaces**: `docs-preview = { run = "vitepress preview docs" }`

---

### `docs:api` — Generate API reference

This task builds all Conventions packages and runs `xmldocmd` for each, then injects Starlight frontmatter.

```toml
"docs:api" = { run = "bash docs/scripts/generate-api.sh", description = "Build Conventions, generate API reference via xmldocmd, inject Starlight frontmatter" }
```

The referenced script `docs/scripts/generate-api.sh` (new file, to be created):

```bash
#!/usr/bin/env bash
set -euo pipefail

OUT=/tmp/conventions-publish
DOCS_API=docs/src/content/docs/api
SOURCE_BASE=https://github.com/RocketSurgeonsGuild/Conventions/blob/main/src

rm -rf "$OUT"
mkdir -p "$OUT"

# Build all packages
dotnet build src/ -c Release --nologo

# Generate API reference for each package
# Each invocation writes to a package-specific subdirectory
declare -A PACKAGES=(
  ["Conventions"]="abstractions"
  ["Conventions.Abstractions"]="abstractions"
  ["Conventions.Analyzers"]="analyzers"
  ["Conventions.Autofac"]="autofac"
  ["Conventions.Configuration.Json"]="configuration-json"
  ["Conventions.Configuration.Yaml"]="configuration-yaml"
  ["Conventions.Diagnostics"]="diagnostics"
  ["Conventions.DryIoc"]="dryioc"
  ["Conventions.Serilog"]="serilog"
  ["CommandLine"]="commandline"
  ["Hosting"]="hosting"
  ["Web.Hosting"]="web-hosting"
  ["WebAssembly.Hosting"]="webassembly-hosting"
  ["Aspire.Hosting"]="aspire-hosting"
  ["Aspire.Hosting.Testing"]="aspire-hosting-testing"
)

for PKG in "${!PACKAGES[@]}"; do
  SUBDIR="${PACKAGES[$PKG]}"
  DLL_DIR="src/$PKG/bin/Release/net10.0"

  # Find the primary DLL (skip .resources.dll, test DLLs, etc.)
  DLL=$(find "$DLL_DIR" -name "Rocket.Surgery.*.dll" -not -name "*.resources.dll" 2>/dev/null | head -1)

  if [[ -z "$DLL" ]]; then
    echo "WARNING: No DLL found for $PKG in $DLL_DIR — skipping" >&2
    continue
  fi

  XML="${DLL%.dll}.xml"
  if [[ ! -f "$XML" ]]; then
    echo "WARNING: No XML doc file for $PKG ($XML) — skipping" >&2
    continue
  fi

  mkdir -p "$DOCS_API/$SUBDIR"
  xmldocmd "$DLL" "$DOCS_API/$SUBDIR/" \
    --source "$SOURCE_BASE/$PKG/" \
    --clean
done

# Inject Starlight frontmatter
node docs/scripts/add-api-frontmatter.mjs
```

---

### `docs:build` — Full production build

```toml
"docs:build" = { run = "mise run docs:api && cd docs && npm run build", description = "Generate API reference then build Starlight site" }
```

**Sequence**:

1. `mise run docs:api` — builds dotnet, generates API reference markdown
2. `cd docs && npm run build` — runs Astro production build (includes link validation)

---

## Invariants

- All tasks operate from the repo root (mise handles the working directory).
- `docs:api` must always run before `docs:build` — the Astro build reads the generated API markdown.
- `xmldocmd` is available on PATH via mise (`"dotnet:xmldocmd" = "2.9.0"` in `[tools]`); no `dotnet tool restore` needed.
- The `net10.0` target is the primary build output; `net8.0` outputs exist only for compat projects.
