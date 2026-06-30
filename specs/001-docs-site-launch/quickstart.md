# Quickstart Validation Guide: Conventions Documentation Site Launch

**Feature**: `specs/001-docs-site-launch`
**Date**: 2026-06-27
**See also**: [data-model.md](data-model.md) | [contracts/mise-tasks.md](contracts/mise-tasks.md) | [contracts/site-identity.md](contracts/site-identity.md) | [contracts/plugin-verification-matrix.md](contracts/plugin-verification-matrix.md)

---

## Prerequisites

```sh
# All tools managed by mise — install once:
mise install

# Node dependencies (npm workspaces; legacy-peer-deps is set in root .npmrc:
# @astrojs/starlight pulls astro 7 while starlight-llms-txt declares an astro 6 peer):
npm install

# Verify xmldocmd is available:
xmldocmd --version   # should print 2.9.0

# Verify Astro is available:
npm run astro --workspace docs -- --version
```

---

## Scenario 1: Dev Server Starts (US3 — P3)

**What this proves**: `mise run docs` works; Starlight replaces VitePress.

```sh
mise run docs
```

**Expected**:

- Terminal: `Local http://localhost:4321/`
- No error on startup
- Browser: Home page loads with title "Conventions" (not "Indago")

**Pass condition**: Site accessible at `http://localhost:4321/`. Home page shows Conventions hero.

---

## Scenario 2: No 404s on Navigation (US1 — P1)

**What this proves**: All sidebar topic links resolve; section landing pages exist.

With the dev server running, open each URL and confirm HTTP 200 (not a Starlight 404 page):

| URL                                | Expected Page                      |
| ---------------------------------- | ---------------------------------- |
| `http://localhost:4321/guides/`    | Getting Started landing page       |
| `http://localhost:4321/concepts/`  | Concepts landing page              |
| `http://localhost:4321/api/`       | API Reference landing page         |
| `http://localhost:4321/changelog/` | Changelog section (plugin-managed) |

**Pass condition**: All 4 URLs return a rendered page, not a 404.

---

## Scenario 3: API Reference Generates (US2 — P2)

**What this proves**: `docs:api` task works; xmldocmd runs for all packages; frontmatter is injected.

```sh
# Ensure a Release build exists:
dotnet build src/ -c Release --nologo

# Generate API reference:
mise run docs:api
```

**Expected**:

- `docs/src/content/docs/api/` directory created with subdirectories per package
- Each subdirectory contains `.md` files
- Each `.md` file starts with `---` (frontmatter injected by `add-api-frontmatter.mjs`)
- WARNING lines in output (not ERROR) for any packages without XML doc files — this is acceptable

**Spot check**:

```sh
# Verify frontmatter on one generated file:
head -5 docs/src/content/docs/api/abstractions/IConventionContext.md
# Should start with:
# ---
# title: "IConventionContext"
# ---
```

Then start the dev server and navigate to `http://localhost:4321/api/` — generated pages should appear in the sidebar.

**Pass condition**: API pages appear in dev server sidebar; at least `IConvention` and `IConventionContext` pages are present with correct titles.

---

## Scenario 4: Production Build Passes Link Validation (US1 — P1)

**What this proves**: No broken internal links; `starlight-links-validator` passes.

```sh
# Requires API reference to be generated first:
mise run docs:build
```

**Expected**:

- Build completes without error
- Terminal includes output from link validator with `0 broken links` (or similar)
- `docs/dist/` directory created

**Pass condition**: `mise run docs:build` exits 0.

---

## Scenario 5: Plugin Verification Walkthrough (US5 — P4)

**What this proves**: All 13 Starlight plugins are working.

Use the [plugin verification matrix](contracts/plugin-verification-matrix.md) as the checklist. For each plugin:

1. Start the dev server: `mise run docs`
2. Navigate to the relevant page or perform the described action
3. Confirm the expected feature is visible/functional
4. Record status in the matrix

**Key verifications** (highest priority):

| Plugin                     | Quick Check                                                                    |
| -------------------------- | ------------------------------------------------------------------------------ |
| `starlight-sidebar-topics` | Sidebar shows "Getting Started / Concepts / API Reference / Changelog" groups  |
| `starlight-github-alerts`  | A page with `> [!NOTE]` shows a styled blue callout                            |
| `starlight-tags`           | `http://localhost:4321/tags/` loads and lists at least one tag                 |
| `starlight-page-actions`   | An "Edit page" link points to `github.com/RocketSurgeonsGuild/Conventions`     |
| `starlight-llms-txt`       | After `npm run build`: `docs/dist/llms.txt` exists and lists pages             |
| `starlight-changelogs`     | `http://localhost:4321/changelog/` renders (may be empty without GH_API_TOKEN) |

**Pass condition**: All 13 plugins marked Working or Fixed in the matrix; none remain Pending.

---

## Scenario 6: CI Simulation (US4 — P3)

**What this proves**: The deploy workflow would succeed; base path is correct for GitHub Pages.

```sh
# Simulate CI environment:
GITHUB_ACTIONS=true mise run docs:build
```

**Expected**:

- Build uses base path `/Conventions` (check output HTML for `/Conventions/` links)
- Build exits 0

**Pass condition**: Build succeeds; inspecting `docs/dist/index.html` shows `/Conventions/` in asset URLs.

---

## Acceptance Summary

| Scenario                              | User Story | Status |
| ------------------------------------- | ---------- | ------ |
| 1. Dev server starts                  | US3        | ✅     |
| 2. No 404s on navigation              | US1        | ✅     |
| 3. API reference generates            | US2        | ✅     |
| 4. Production build + link validation | US1        | ✅     |
| 5. Plugin verification (all 13)       | US5        | ✅     |
| 6. CI base-path simulation            | US4        | ✅     |

All 6 scenarios must pass before the feature is complete.
