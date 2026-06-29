# Tasks: Conventions Documentation Site Launch

**Input**: Design documents from `specs/001-docs-site-launch/`

**Prerequisites**: [plan.md](plan.md) | [spec.md](spec.md) | [research.md](research.md) | [data-model.md](data-model.md) | [contracts/mise-tasks.md](contracts/mise-tasks.md) | [contracts/site-identity.md](contracts/site-identity.md) | [contracts/plugin-verification-matrix.md](contracts/plugin-verification-matrix.md) | [quickstart.md](quickstart.md)

**Tests**: No dedicated test tasks — this is a documentation site feature. Validation is performed via `npm run build`, the link validator, and the quickstart.md scenarios.

**Organization**: Tasks grouped by user story to enable independent implementation and validation.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (US1–US5)

---

## Phase 1: Setup (Site Identity Updates)

**Purpose**: Replace all Indago references with Conventions in config files. These are prerequisites for every subsequent phase — the dev server will show wrong branding until this is done.

- [x] T001 Update `docs/astro.config.mjs`: change base path from `'/Indago'` to `'/Conventions'`, title to `'Conventions'`, description to Conventions description, social link to `https://github.com/RocketSurgeonsGuild/Conventions`, and editLink to Conventions URL (see `contracts/site-identity.md`)
- [x] T002 [P] Update `docs/astro.config.mjs` sidebar: replace the 5 Indago topics (guide/reference/architecture/api/changelog) with 4 Conventions topics (Getting Started→guides/, Concepts→concepts/, API Reference→api/, Changelog) per `contracts/site-identity.md`
- [x] T003 [P] Update `docs/src/content.config.ts`: change `repo: 'Indago'` to `repo: 'Conventions'` in the `changelogsLoader` config
- [x] T004 [P] Update `docs/src/content/docs/index.mdx`: rewrite hero frontmatter with Conventions title, tagline, description, GitHub link, and banner text per `contracts/site-identity.md`
- [x] T005 [P] Update `docs/package.json`: change `name` from `"indago-docs"` to `"conventions-docs"`
- [x] T006 [P] Update `docs/tags.yml`: add Conventions-specific tags (hosting, configuration, logging, autofac, dryioc, aspire) per `data-model.md` tag table; retain tags already appropriate for Conventions (aot, source-generator, getting-started, api, di, architecture)
- [x] T007 [P] Update `.vscode/extensions.json`: add `"HiDeoo.starlight-links"` to the recommendations array

---

## Phase 2: Foundational (Mise Tasks + Dev Server)

**Purpose**: Update mise task definitions so `mise run docs` works with Astro instead of VitePress. This blocks all user story validation — no story can be tested until the dev server runs.

**⚠️ CRITICAL**: No user story validation can proceed until the dev server starts successfully.

- [x] T008 Update `.config/mise.toml`: replace `docs = { run = "vitepress dev docs" }` with `docs = { run = "cd docs && npm run dev", description = "Start Astro/Starlight dev server" }`
- [x] T009 [P] Update `.config/mise.toml`: replace `docs-preview = { run = "vitepress preview docs" }` with `"docs:preview" = { run = "cd docs && npm run preview", description = "Preview built Starlight site" }`
- [x] T010 [P] Update `.config/mise.toml`: add `"docs:build" = { run = "cd docs && npm run build", description = "Build Starlight site (stub — updated in US2 to include docs:api)" }`
- [x] T011 Run `cd docs && npm install` (or `mise run docs` which triggers install) and confirm no dependency errors
- [x] T012 Run `mise run docs` and confirm Astro dev server starts at `http://localhost:4321` with Conventions title in browser header

**Checkpoint**: Dev server running — user story validation can now begin.

---

## Phase 3: User Story 1 — No 404s on Navigation (Priority: P1) 🎯 MVP

**Goal**: Every primary sidebar topic resolves to a rendered page. Zero 404s. Zero broken internal links on a production build.

**Independent Test**: With dev server running, navigate to `/guide/`, `/concepts/`, `/api/`, `/changelog/` — each returns a rendered page, not a 404. Then run `cd docs && npm run build` and confirm it exits 0 with zero link errors.

- [x] T013 [US1] Create `docs/src/content/docs/concepts/index.md` — section landing page with links to all 5 concept pages (introduction, defining-conventions, convention-context, source-generation, unit-tests)
- [x] T014 [P] [US1] Create `docs/src/content/docs/api/index.md` — placeholder landing page introducing the API Reference section (note: actual package links added in US2 after xmldocmd runs)
- [x] T015 [P] [US1] Verify `docs/src/content/docs/guides/index.md` exists and has appropriate Conventions-specific content (not Indago); update if needed
- [x] T016 [US1] Navigate to `http://localhost:4321/guide/` in browser — confirm renders (HTTP 200, not Starlight 404 page) [validated via build: fixed /guide/ → /guides/ in astro.config.mjs and index.mdx; all internal links valid]
- [x] T017 [US1] Navigate to `http://localhost:4321/concepts/` in browser — confirm renders (HTTP 200, section landing page) [validated via build: "All internal links are valid."]
- [x] T018 [P] [US1] Navigate to `http://localhost:4321/api/` in browser — confirm renders (HTTP 200, placeholder landing page) [validated via build: "All internal links are valid."]
- [x] T019 [US1] Run `cd docs && npm run build` — confirm exits 0 and link validator reports zero broken links [PASSED: 178 pages built, "All internal links are valid."]

**Checkpoint**: US1 complete — all nav links resolve, production build passes.

---

## Phase 4: User Story 2 — API Reference Generation (Priority: P2)

**Goal**: All public Conventions types auto-generated into browsable API reference pages. Zero manual steps after `dotnet build`.

**Independent Test**: Run `dotnet build src/ -c Release --nologo` then `mise run docs:api`. Open `http://localhost:4321/api/` — generated package subdirectories appear in sidebar and at least `IConvention` and `IConventionContext` pages render with summaries.

- [x] T020 [US2] Create `docs/scripts/generate-api.sh` — bash script that builds all src/ packages, runs xmldocmd for each of the 15 packages (all except `Conventions.Analyzers.roslyn4.8` variant), and invokes `node docs/scripts/add-api-frontmatter.mjs` (see `contracts/mise-tasks.md` for full script body)
- [x] T021 [US2] Make `docs/scripts/generate-api.sh` executable: `chmod +x docs/scripts/generate-api.sh`
- [x] T022 [US2] Update `.config/mise.toml`: add `"docs:api" = { run = "bash docs/scripts/generate-api.sh", description = "Build Conventions, generate API reference via xmldocmd, inject Starlight frontmatter" }`
- [x] T023 [US2] Update `.config/mise.toml`: update `"docs:build"` to `{ run = "mise run docs:api && cd docs && npm run build", description = "Generate API reference then build Starlight site" }` (replaces stub from T010)
- [x] T024 [US2] Run `dotnet build src/ -c Release --nologo` to produce all DLLs and XML doc files
- [x] T025 [US2] Run `mise run docs:api` and confirm: `docs/src/content/docs/api/` subdirectories created, `.md` files present, each file starts with `---` (frontmatter injected)
- [x] T026 [US2] Update `docs/src/content/docs/api/index.md` with actual package links for each generated subdirectory (replacing the placeholder from T014)
- [x] T027 [US2] Start dev server (`mise run docs`) and navigate to `http://localhost:4321/api/` — confirm generated package subdirectories appear in "API Reference" sidebar group
- [x] T028 [P] [US2] Navigate to `http://localhost:4321/api/abstractions/` (or equivalent) and confirm `IConvention` and `IConventionContext` pages render with type summaries
- [x] T029 [P] [US2] Navigate to a generated method page and confirm parameter names, types, and descriptions display correctly

**Checkpoint**: US2 complete — API reference auto-generates and appears in the live site.

---

## Phase 5: User Story 3 — Full Mise Task Suite (Priority: P3)

**Goal**: `mise run docs`, `mise run docs:api`, `mise run docs:build`, and `mise run docs:preview` all work correctly from a clean checkout.

**Independent Test**: Run `mise run docs:build` — exits 0, produces `docs/dist/`. Run `mise run docs:preview` — preview server starts. These tasks already exist from Phase 2 and 4; this phase validates them end-to-end.

- [x] T030 [US3] Run `mise run docs:build` end-to-end (includes docs:api + Astro build + link validation) and confirm it exits 0
- [x] T031 [US3] Run `mise run docs:preview` after a successful build — confirm preview server starts and site is browsable at `http://localhost:4321`
- [x] T032 [P] [US3] Verify hot-reload: with `mise run docs` running, edit any `.md` file under `docs/src/content/docs/` and confirm browser updates without full restart

**Checkpoint**: US3 complete — all mise docs tasks work correctly.

---

## Phase 6: User Story 4 — CI Deployment (Priority: P3)

**Goal**: Push to main triggers a GitHub Actions workflow that builds and deploys the site to GitHub Pages at `/Conventions`.

**Independent Test**: Create `.github/workflows/deploy-docs.yml` using the Indago reference pattern, confirm it passes lint (`actionlint`), and run `GITHUB_ACTIONS=true mise run docs:build` to verify base path `/Conventions` appears in generated HTML.

- [x] T033 [US4] Create `.github/workflows/deploy-docs.yml`: deploy workflow that triggers on push to main (paths: `docs/**`, `src/**`) and `workflow_dispatch`; job: checkout → setup .NET 10.0.x → setup Node 22 → `dotnet build src/ -c Release --nologo` → install xmldocmd (`dotnet tool install -g xmldocmd --version 2.9.0`) → `mise run docs:api` → `cd docs && npm ci && npm run build` (with `GH_API_TOKEN` secret) → `actions/upload-pages-artifact@v3` (path: `docs/dist`) → `actions/deploy-pages@v4` (see Indago `.github/workflows/deploy-docs.yml` as template)
- [x] T034 [US4] Archive or delete `.github/workflows/publish-docs.yml` (old VitePress artifact download pattern — superseded by deploy-docs.yml)
- [x] T035 [US4] Run `GITHUB_ACTIONS=true mise run docs:build` locally and confirm `docs/dist/index.html` contains `/Conventions/` in asset URLs (verifies base path is `/Conventions` in CI mode)
- [x] T036 [P] [US4] Run `actionlint .github/workflows/deploy-docs.yml` (if available) to validate workflow syntax

**Checkpoint**: US4 complete — CI workflow ready for deployment.

---

## Phase 7: User Story 5 — Plugin Verification (Priority: P4)

**Goal**: All 13 installed Starlight plugins are confirmed working with representative content. Zero plugins remain in "Unknown" state in the verification matrix.

**Independent Test**: Walk through the plugin verification matrix in `contracts/plugin-verification-matrix.md`. Every plugin must be marked Working, Fixed, or Removed (with reason).

- [x] T037 [US5] Add a `> [!NOTE]` GitHub alert callout to `docs/src/content/docs/guides/index.md` and verify it renders as a styled callout in the browser (validates `starlight-github-alerts`)
- [x] T038 [P] [US5] Add `tags: [getting-started]` to frontmatter of `docs/src/content/docs/guides/index.md`; add `tags: [source-generator, aot]` to `docs/src/content/docs/concepts/source-generation.md`; navigate to `http://localhost:4321/tags/` and confirm tag index loads with entries (validates `starlight-tags`)
- [x] T039 [P] [US5] Add a heading badge to a concept page: e.g. in `docs/src/content/docs/concepts/source-generation.md` add `## How It Works [New in v8]` syntax; verify badge renders (validates `starlight-heading-badges`)
- [x] T040 [P] [US5] Add at least one image or diagram to a content page (e.g. architecture overview in `docs/src/content/docs/concepts/introduction.md`); click image in browser and confirm zoom/lightbox opens (validates `starlight-image-zoom`)
- [x] T041 [US5] Navigate to `http://localhost:4321/concepts/introduction` (a long page) and scroll down; confirm scroll-to-top button appears and clicking it works (validates `starlight-scroll-to-top`)
- [x] T042 [P] [US5] Navigate to any content page and confirm "Edit page" action link points to `https://github.com/RocketSurgeonsGuild/Conventions/edit/main/docs/src/content/docs/` (validates `starlight-page-actions`)
- [x] T043 [P] [US5] Add `<Icon name="github" />` or equivalent icon component to `docs/src/content/docs/index.mdx`; confirm icon renders in browser (validates `starlight-plugin-icons`)
- [x] T044 [US5] Verify `starlight-auto-drafts`: add `draft: true` to any page frontmatter; run `cd docs && npm run build`; confirm the page does NOT appear in `docs/dist/` navigation or search
- [x] T045 [US5] Navigate to `http://localhost:4321/changelog/`; confirm page renders without build error even without `GH_API_TOKEN` (validates `starlight-changelogs` degrades gracefully)
- [x] T046 [US5] Run `cd docs && npm run build` and confirm `docs/dist/llms.txt` exists and lists at least the home page and one guide page (validates `starlight-llms-txt`)
- [x] T047 [US5] Confirm `starlight-sidebar-topics` is working: sidebar shows "Getting Started / Concepts / API Reference / Changelog" topic group labels (not a flat sidebar)
- [x] T048 [US5] Confirm `starlight-links-validator` passes: production build exits 0 with zero broken link errors (already validated in US1/US3 but explicitly record as plugin verification)
- [x] T049 [US5] Update `contracts/plugin-verification-matrix.md`: record Working/Fixed/Removed status and evidence for all 13 plugins

**Checkpoint**: US5 complete — all plugins verified, matrix fully populated.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Formatting, cleanup, and final validation across all stories.

- [x] T050 Remove VitePress-specific `toc.yml` files from `docs/src/content/docs/concepts/` and `docs/src/content/docs/guides/` (Starlight uses autogenerate; toc.yml is ignored but adds noise)
- [x] T051 [P] Run `prettier --write docs/` on all documentation source files and confirm no formatting changes are needed (or apply them and confirm)
- [x] T052 [P] Audit all existing content pages (`concepts/*.md`, `guides/*.md`) for any remaining Indago-specific references (project name, GitHub URLs, descriptions) and update to Conventions
- [x] T053 Run full `quickstart.md` validation — all 6 scenarios must pass:
    - Scenario 1: Dev server starts (`mise run docs`)
    - Scenario 2: No 404s on all 4 nav topics
    - Scenario 3: API reference generates (`mise run docs:api` + spot-check pages)
    - Scenario 4: Production build passes link validation (`mise run docs:build`)
    - Scenario 5: Plugin verification matrix complete (all Working/Fixed/Removed)
    - Scenario 6: CI base-path simulation (`GITHUB_ACTIONS=true mise run docs:build`)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 completion (needs updated config) — **BLOCKS all user story validation**
- **US1 (Phase 3)**: Depends on Phase 2 (dev server must be running for browser verification)
- **US2 (Phase 4)**: Depends on Phase 3 (api/index.md placeholder must exist before xmldocmd output is added)
- **US3 (Phase 5)**: Depends on Phase 4 (docs:build must include docs:api)
- **US4 (Phase 6)**: Depends on Phase 3 (site must build cleanly for CI workflow to be meaningful)
- **US5 (Phase 7)**: Depends on Phase 3 (navigable site required to verify plugins across pages)
- **Polish (Phase 8)**: Depends on all user story phases

### User Story Dependencies

- **US3 and US4 can proceed in parallel once US1 is complete**
- **US5 can proceed once US1 is complete** (doesn't require US2's API reference for most plugin checks)
- **US2 depends only on the Foundational phase** — can start as soon as the dev server works

### Parallel Opportunities Per Phase

**Phase 1** (T002–T007 all parallel after T001):

```
T001 (astro.config.mjs identity) → T002 (sidebar), T003 (content.config.ts),
                                    T004 (index.mdx), T005 (package.json),
                                    T006 (tags.yml), T007 (.vscode/extensions.json)
```

**Phase 3** (T013–T015 parallel, then T016–T019 sequential):

```
T013 (concepts/index.md) ┐
T014 (api/index.md)      ├→ T016-T019 (browser checks + build)
T015 (guides/index.md)   ┘
```

**Phase 4** (T020–T023 sequential, then T025–T026, then T027–T029):

```
T020 → T021 → T022 → T023 → T024 → T025 → T026 → T027 → T028 ║ T029
```

**Phase 7** (T037 sequential to anchor, T038–T048 largely parallel):

```
T037 → T038 ║ T039 ║ T040 ║ T041 ║ T042 ║ T043 → T044 → T045 → T046 → T047 → T048 → T049
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Identity updates
2. Complete Phase 2: Mise tasks + dev server
3. Complete Phase 3: No 404s (US1)
4. **STOP and VALIDATE**: Run `mise run docs` + browser check all nav links + `npm run build`
5. The site is navigable and deployable even without API reference

### Incremental Delivery

1. Phase 1 + 2 → Site starts with Conventions identity
2. Phase 3 (US1) → Navigable site, no 404s (MVP!)
3. Phase 4 (US2) → API reference auto-generates
4. Phase 5 (US3) → Full mise pipeline validated
5. Phase 6 (US4) → CI deployment live
6. Phase 7 (US5) → All plugins verified
7. Phase 8 → Polish complete, site launched

---

## Notes

- `[P]` tasks operate on different files with no in-flight dependencies — safe to run in parallel
- `[Story]` label maps each task to the spec user story for traceability
- No library source code (`src/`) should be modified by any task — all changes are under `docs/`, `.config/mise.toml`, `.github/workflows/`, and `.vscode/`
- The `Conventions.Analyzers.roslyn4.8` project is a Roslyn variant, not a separate NuGet package — skip it in `generate-api.sh`
- The `add-api-frontmatter.mjs` script is idempotent (skips files that already have frontmatter) — safe to re-run
- Commit after each phase checkpoint to preserve progress
