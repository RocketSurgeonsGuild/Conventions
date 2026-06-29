# Implementation Plan: Conventions Documentation Site Launch

**Branch**: `feature/docs` | **Date**: 2026-06-27 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-docs-site-launch/spec.md`

## Summary

Launch the Conventions documentation site by: updating the existing Astro Starlight configuration to use the Conventions identity; wiring up `xmldocmd` API reference generation for all 16 src packages; updating mise tasks from VitePress to Astro; adding missing section landing pages; fixing the CI deployment workflow; and verifying all 13 installed Starlight plugins produce visible output on the live site.

The Indago repository (specs 002 and 003) is the proven reference implementation. The pattern is fully established there — this plan adapts it to the Conventions multi-package layout.

---

## Technical Context

**Language/Version**: Node.js 22 (docs toolchain); C# / .NET 10 (source assemblies for API reference)

**Primary Dependencies**:

- Astro 7 + `@astrojs/starlight` 0.41 (already installed in `docs/package.json`)
- 13 Starlight plugins (all already in `docs/package.json` — see Plugin Inventory in research.md)
- `xmldocmd` 2.9.0 (already in `.config/mise.toml`)
- `prettier` 3.8 (formatter, already in mise)

**Storage**: Static files only. API reference generated to `docs/src/content/docs/api/`. Changelog pulled from GitHub Releases at build time via `starlight-changelogs`.

**Testing**: No unit tests. Validation via: `npm run build` (Astro production build), `starlight-links-validator` (link checks), and manual browser walkthrough per `quickstart.md`.

**Target Platform**: GitHub Pages static site; also served locally via `astro dev`.

**Project Type**: Documentation site (pure docs, no library code changes).

**Performance Goals**: Production build completes in ≤ 5 minutes (including API generation for all 16 packages). Dev server hot-reload within 2 seconds.

**Constraints**:

- Zero changes to library source (`src/`), the Roslyn generator, tests, or public API files.
- RS0017 public API tracking is not involved — this is docs-only.
- Prettier must pass on all docs source files.
- All 16 src packages ship XML docs (`<GenerateDocumentationFile>true</GenerateDocumentationFile>` in `src/Directory.Build.props`).

**Scale/Scope**: 16 publishable packages → 16 xmldocmd invocations → one API reference section organized by namespace.

---

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design._

| Principle                             | Relevant?                                                           | Status                        |
| ------------------------------------- | ------------------------------------------------------------------- | ----------------------------- |
| I. Convention-First, Reflection-Never | No — docs-only feature                                              | ✅ Not applicable             |
| II. Abstractions Drive Everything     | No — no library code changes                                        | ✅ Not applicable             |
| III. Public API Surface (RS0017)      | No — docs-only, no API changes                                      | ✅ Not applicable             |
| IV. Central Package Management        | No — NuGet unchanged; npm `docs/package.json` is a separate package | ✅ Not applicable             |
| V. Snapshot Tests Guard the Generator | No — no generator changes                                           | ✅ Not applicable             |
| Formatting gate                       | Yes — all docs source files must pass `prettier`                    | ✅ Enforced in CI             |
| Build gate                            | Yes — docs build must pass in CI                                    | ✅ Enforced via docs workflow |

**Verdict**: No constitution violations. This feature is docs-only and does not touch any code gated by the Conventions governance model.

---

## Project Structure

### Documentation (this feature)

```text
specs/001-docs-site-launch/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
└── tasks.md             # Phase 2 output (/speckit-tasks)
```

### Source Code Layout (docs feature scope)

```text
docs/                               # Astro/Starlight documentation site
├── astro.config.mjs                # NEEDS UPDATE: Indago → Conventions identity + sidebar
├── package.json                    # Already correct plugin versions; minor name update
├── tsconfig.json                   # Already present
├── tags.yml                        # NEEDS UPDATE: Indago-specific tags → Conventions tags
├── scripts/
│   └── add-api-frontmatter.mjs     # Already present; no changes needed
└── src/
    ├── content.config.ts           # NEEDS UPDATE: changelog repo Indago → Conventions
    └── content/
        └── docs/
            ├── index.mdx           # NEEDS UPDATE: Indago hero → Conventions hero
            ├── concepts/           # EXISTS: 5 pages; toc.yml needs removal (Starlight auto-nav)
            │   └── index.md        # NEEDS CREATION: section landing page
            ├── guides/             # EXISTS: 2 pages + index.md (landing already present)
            └── api/                # NEEDS CREATION: xmldocmd output per package

.config/mise.toml                   # NEEDS UPDATE: docs/docs-preview tasks; add docs:api, docs:build
.github/workflows/publish-docs.yml  # NEEDS UPDATE: VitePress artifact → Astro build + deploy pattern
.vscode/extensions.json             # NEEDS UPDATE: add starlight-links extension recommendation
```

**Structure Decision**: The Conventions docs follow the same layout as Indago with one difference — `concepts/` replaces the Indago `architecture/` and `reference/` sections, reflecting the content already migrated. The sidebar topics will be: Getting Started (`guides/`), Concepts (`concepts/`), API Reference (`api/`), Changelog.

---

## Complexity Tracking

No constitution violations requiring justification.

---

## Plugin Inventory (all 13 confirmed installed)

| Plugin                          | Purpose                             | Activation Required                          |
| ------------------------------- | ----------------------------------- | -------------------------------------------- |
| `starlight-auto-drafts`         | Exclude draft pages from production | Set `draft: true` in any page frontmatter    |
| `starlight-github-alerts`       | GitHub-style alert callouts         | Use `> [!NOTE]` syntax in markdown           |
| `starlight-sidebar-topics`      | Topic-grouped sidebar               | Config in `astro.config.mjs`                 |
| `starlight-links-validator`     | Build-fail on broken links          | Automatic                                    |
| `starlight-heading-badges`      | Version/status badges on headings   | Use `## Title [badge]` syntax                |
| `starlight-image-zoom`          | Click-to-zoom images                | Automatic on all images                      |
| `starlight-scroll-to-top`       | Scroll-to-top button                | Automatic on long pages                      |
| `starlight-page-actions`        | Edit-on-GitHub button               | Config `editLink` in `astro.config.mjs`      |
| `starlight-plugin-icons`        | Icons in MDX content                | Use `<Icon>` component                       |
| `starlight-tags`                | Content tagging + tag index         | Set `tags:` in page frontmatter              |
| `starlight-changelogs`          | GitHub Releases changelog           | Config `owner`/`repo` in `content.config.ts` |
| `starlight-llms-txt`            | Auto-generate `llms.txt`            | Automatic on build                           |
| `starlight-links` (VS Code ext) | Editor link autocomplete            | Add to `.vscode/extensions.json`             |
