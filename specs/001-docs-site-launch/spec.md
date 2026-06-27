# Feature Specification: Conventions Documentation Site Launch

**Feature Branch**: `feature/docs`

**Created**: 2026-06-27

**Status**: Draft

**Input**: User description: "I'm modernizing my documentation and would like you to help get it up and running. I want to generate the docs using starlight, which has been started. I also want to generate api documentation using xmldocmd. I have done some work already in Indago, it's available in the folder /Users/david/Development/RocketSurgeonsGuild/Indago. It has a squad, specify specs related to documentation as well, and is currently iterating on some of the issues of the doc site."

## Overview

The Conventions repository already has an Astro Starlight docs site started under `docs/` — the tooling is installed, the plugin set is configured, and several content pages have been migrated. However, the site is not yet launch-ready: the configuration still references the Indago project, the `mise` tasks still point to the old VitePress commands, section landing pages are missing, the API reference generation pipeline is not wired in, and there is no CI deployment workflow.

This feature brings the Conventions documentation site from its current "started but broken" state to a fully functional, deployed site with automated API reference generation from Conventions' public assemblies.

The Indago project (specs 002 and 003) serves as the reference implementation and has already resolved the same class of problems for a similar library. Conventions can adopt the same patterns and plugin set, adapted for its identity, structure, and assembly surface.

---

## User Scenarios & Testing _(mandatory)_

### User Story 1 — Developer Navigates the Live Site Without Hitting 404s (Priority: P1)

A .NET developer discovers the Conventions documentation site and clicks through the primary navigation topics — Getting Started, Concepts, Reference, API Reference, and Changelog. Every topic lands on a real page that orients them and links into the section. No 404s, no Starlight error pages, no dead links.

**Why this priority**: A broken navigation is the most visible defect a newly launched docs site can have. Until every top-level link resolves, the site cannot be considered launched. This is the minimum viable deliverable.

**Independent Test**: With the dev server running, navigate to each sidebar topic link and confirm it returns a rendered page, not a 404. Then run a production build and confirm the link validator reports zero errors.

**Acceptance Scenarios**:

1. **Given** the docs site is running, **When** a reader clicks the "Getting Started" topic, **Then** a landing page renders (HTTP 200) and introduces the section with links to its content.
2. **Given** the docs site is running, **When** a reader clicks any of the currently-404 section topics (Reference, API Reference), **Then** a landing page renders (HTTP 200) introducing that section.
3. **Given** a production build completes, **When** the link validator runs, **Then** zero broken internal links are reported.
4. **Given** the docs site, **When** any content page is opened, **Then** the site header shows "Conventions" (not "Indago") and all GitHub links point to the Conventions repository.

---

### User Story 2 — Developer Finds API Documentation for a Public Type (Priority: P2)

A .NET developer using the Conventions library needs to look up the API surface for a specific convention type, interface, or attribute. They navigate to the API Reference section and find complete documentation generated from the published assemblies and their XML doc files — summaries, parameters, return types, and remarks — without reading source code.

**Why this priority**: API reference is the most-visited section of any library documentation site. Conventions has a large public API surface across many packages; without auto-generated reference, developers must rely on IntelliSense alone.

**Independent Test**: Run the API reference generation step against the built Conventions assemblies and verify that all public types appear in the rendered site with correct summaries and signatures.

**Acceptance Scenarios**:

1. **Given** a developer visits the API Reference section, **When** they browse the generated pages, **Then** all public types from the Conventions assemblies are listed with their XML doc summaries and method/property signatures.
2. **Given** a public type with `<param>` and `<returns>` XML doc comments, **When** its API page renders, **Then** parameter names, types, and descriptions display correctly.
3. **Given** the `mise run docs` command is run, **When** the build completes, **Then** the API reference is regenerated automatically — no manual step required.
4. **Given** an assembly that has no XML doc file, **When** the generation runs, **Then** it skips that assembly with a warning rather than failing the entire build.

---

### User Story 3 — Maintainer Runs the Docs Locally with Mise (Priority: P3)

A maintainer runs `mise run docs` and the Starlight dev server starts up, reflecting content changes on save. They can author new content, verify plugin behavior in the browser, and run `mise run docs-build` to get a production build with link validation. No VitePress commands, no confusion about which tool to use.

**Why this priority**: The `mise` tasks are the team's agreed-upon interface for all development tasks. They must be updated to serve the Starlight site, not the deleted VitePress site. Without this, the documented workflow is broken.

**Independent Test**: Run `mise run docs` on a clean checkout and confirm the Starlight dev server starts. Run `mise run docs-build` and confirm a production build succeeds with the link validator passing.

**Acceptance Scenarios**:

1. **Given** a clean checkout with all dependencies installed, **When** `mise run docs` is run, **Then** the Starlight dev server starts and the site is accessible locally within 2 minutes.
2. **Given** the dev server running, **When** a content file is modified, **Then** the browser reflects the change without a full restart.
3. **Given** `mise run docs-build` is run, **When** the build completes, **Then** a static site is produced in `docs/dist/` and no broken links are reported.
4. **Given** `mise run docs-preview` is run after a build, **Then** the pre-built static site is served locally for verification.

---

### User Story 4 — CI Deploys the Site on Push to Main (Priority: P3)

When a PR merges to main and documentation is affected, CI automatically builds and deploys the updated Conventions docs site to GitHub Pages. The deployment URL uses the `Conventions` base path. The CI job for docs is independent of the library build — docs-only PRs can be validated and merged faster.

**Why this priority**: Continuous deployment is a constitutional requirement for documentation. Without it, the site goes stale. This mirrors the existing pattern from the Indago repository.

**Independent Test**: A CI run triggered by a documentation change builds the static site (with API reference generation) and deploys to GitHub Pages without manual steps.

**Acceptance Scenarios**:

1. **Given** a commit is pushed that modifies any file under `docs/`, **When** CI runs, **Then** the docs build job triggers and produces a deployable static artifact.
2. **Given** CI is running in GitHub Actions, **When** the docs build runs, **Then** the base path is set to `/Conventions` (not `/Indago`) so all asset and navigation links resolve correctly on GitHub Pages.
3. **Given** a docs build that has a broken internal link, **When** CI runs, **Then** the deployment fails with a clear error before any deployment occurs.
4. **Given** a successful docs build, **When** the deploy step runs, **Then** the site is published to GitHub Pages and accessible at the expected URL.

---

### User Story 5 — Reader Experiences Full Plugin Feature Set (Priority: P4)

A reader browsing the Conventions docs benefits from the full plugin feature set: GitHub-style alert callouts, image zoom on architecture diagrams, a scroll-to-top button on long reference pages, content tagged and browsable by tag, and page action buttons for editing on GitHub. The site responds to OS dark mode preference automatically.

**Why this priority**: These are quality-of-life plugins that are already installed and configured but not verified working for Conventions. They need minimal content/config to activate and should be confirmed working rather than silently installed-but-inert.

**Independent Test**: Open the live site and exercise each plugin feature: click an image to zoom, verify a GitHub alert renders, check that dark mode activates, browse the tag index, click a page action button.

**Acceptance Scenarios**:

1. **Given** a documentation page with an architecture diagram, **When** a reader clicks the image, **Then** a zoomed view opens.
2. **Given** a Markdown blockquote using GitHub alert syntax (`> [!NOTE]`), **When** the page renders, **Then** it displays as a styled callout.
3. **Given** the OS is set to dark mode, **When** the site is opened, **Then** it renders in dark mode automatically.
4. **Given** a reader on any content page, **When** they click the edit link in the page actions area, **Then** they are taken to the correct GitHub edit URL for that file in the Conventions repository.
5. **Given** a content page tagged with one or more tags, **When** a reader browses the tag index, **Then** they see all pages sharing that tag.

---

### Edge Cases

- What happens when the Conventions site is deployed to GitHub Pages with base `/Conventions`? All internal links, asset references, and social links must use the correct base path; Indago-specific absolute links must not appear.
- What happens when xmldocmd runs against a Conventions assembly that has no XML doc file? The generation step must skip gracefully with a warning, not fail the build.
- What happens to content pages that were in the old VitePress site but not yet migrated? They must be tracked and either migrated or explicitly noted as out of scope.
- What happens when a sidebar topic link has no matching landing page? The Starlight 404 page appears. All section roots must have an index page before launch.
- What happens when the changelog plugin tries to fetch GitHub Releases in local dev without a token? It must degrade gracefully without failing the dev server start.
- What happens when a draft page is linked from a non-draft page? The link validator must flag this as a build error.

---

## Requirements _(mandatory)_

### Functional Requirements

**Site Identity & Configuration**

- **FR-001**: The site title, description, social links, and edit-page URL MUST reference the Conventions repository, not Indago.
- **FR-002**: The base path MUST be `/Conventions` in GitHub Actions and `/` locally, matching the GitHub Pages deployment URL for this repository.
- **FR-003**: The site's GitHub social link MUST point to `https://github.com/RocketSurgeonsGuild/Conventions`.

**Mise Task Integration**

- **FR-004**: The `mise run docs` task MUST start the Astro Starlight dev server (replacing the current `vitepress dev docs` command).
- **FR-005**: A `mise run docs-build` task MUST run the production Astro build including API reference generation.
- **FR-006**: A `mise run docs-preview` task MUST serve the production build for local verification.
- **FR-007**: The docs tasks MUST run from the `docs/` directory context so Astro commands work correctly.

**Section Navigation & Landing Pages**

- **FR-008**: Every primary sidebar topic MUST link to a section landing page that renders (HTTP 200) and orients the reader with links into the section.
- **FR-009**: The sidebar structure MUST be organized around the Conventions project's actual content areas (Getting Started, Concepts, Reference, API Reference, Changelog) rather than Indago's structure.
- **FR-010**: The changelog section MUST use the `starlight-changelogs` plugin configured for the `RocketSurgeonsGuild/Conventions` GitHub repository.

**API Reference Generation**

- **FR-011**: The API reference MUST be auto-generated from compiled Conventions assemblies and their XML documentation files using `xmldocmd` (already installed via mise).
- **FR-012**: The generation script MUST run as part of the docs build pipeline so the reference is always in sync with the compiled output.
- **FR-013**: The `add-api-frontmatter.mjs` script (already present) MUST be integrated into the build pipeline to inject Starlight-compatible frontmatter into the generated Markdown files.
- **FR-014**: The API reference MUST cover all public packages in the Conventions repository that ship XML documentation.
- **FR-015**: The generation step MUST skip assemblies without XML doc files gracefully, logging a warning rather than failing the build.

**Content Migration & Quality**

- **FR-016**: All content pages that currently exist under `docs/src/content/docs/` MUST have their frontmatter and links updated to reference Conventions (not Indago).
- **FR-017**: The link validator (`starlight-links-validator`) MUST pass with zero errors on a production build.
- **FR-018**: All documentation source files MUST pass the existing Prettier formatting gate.

**CI Deployment**

- **FR-019**: A GitHub Actions workflow MUST build and deploy the Starlight site to GitHub Pages on push to the main branch.
- **FR-020**: The CI workflow MUST run the dotnet build (to produce assemblies and XML docs) before the docs build, so API reference generation has up-to-date inputs.
- **FR-021**: The CI workflow MUST fail before deployment if the docs build has broken links or other errors.
- **FR-022**: The `starlight-llms-txt` plugin MUST generate an `llms.txt` file listing all non-draft documentation pages at each deployment.

**Plugin Verification**

- **FR-023**: Every installed Starlight plugin MUST be verified working on the live dev server, with representative content added where required to activate the plugin's feature.

### Key Entities

- **Documentation Site**: The Astro Starlight site under `docs/`, serving as the primary documentation for the Conventions library ecosystem.
- **Content Page**: An authored Markdown or MDX file under `docs/src/content/docs/` representing a guide, concept, or reference page.
- **API Reference Page**: A Markdown file auto-generated by `xmldocmd` from a Conventions assembly and its XML doc file, representing one public type or namespace.
- **Section Landing Page**: The index page at each sidebar topic root (`/guide/`, `/concepts/`, `/reference/`, `/api/`) that introduces the section.
- **Mise Task**: A command registered in `.config/mise.toml` that provides the standard developer interface for docs operations.
- **Changelog**: Release notes pulled from the `RocketSurgeonsGuild/Conventions` GitHub Releases API via `starlight-changelogs`.
- **llms.txt**: A machine-readable index of all public documentation pages for AI tool discovery.

---

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of primary sidebar topics resolve to a rendered page — zero return a 404 on the live site.
- **SC-002**: The link validator reports zero broken internal links on a production build.
- **SC-003**: `mise run docs` starts the Starlight dev server successfully on a clean checkout within 2 minutes.
- **SC-004**: `mise run docs-build` completes successfully and produces a static site that passes link validation.
- **SC-005**: The API Reference section covers 100% of public Conventions packages that ship XML documentation, each with at least a type name and summary.
- **SC-006**: Zero manual steps are required to update the API reference after `dotnet build` — it regenerates automatically as part of the docs build pipeline.
- **SC-007**: The site is deployed to GitHub Pages at the expected Conventions URL with no Indago references visible to readers.
- **SC-008**: All 13 installed Starlight plugins are verified working (or have a recorded reason for removal), with zero plugins in an unknown/unverified state.
- **SC-009**: The `llms.txt` file is present and lists all non-draft documentation pages on the deployed site.
- **SC-010**: All documentation source files pass `prettier --write .` with no changes (formatting gate passes).

---

## Assumptions

- The Starlight plugin set and dependency versions already installed in `docs/package.json` are the correct ones to use; no additional plugins need to be evaluated for this launch.
- `xmldocmd` (version 2.9.0) is already installed via mise and available on PATH in both local and CI environments — no additional tool installation is required.
- The existing `add-api-frontmatter.mjs` script is the correct approach for injecting Starlight frontmatter into xmldocmd output; no changes to its logic are anticipated.
- Content pages already migrated to `docs/src/content/docs/` (concepts, guides, index.mdx) need their Indago-specific references updated but do not require structural rewrites.
- The old VitePress docs content (visible as deleted files in the git diff) has already been superseded by the Astro migration; no VitePress content needs to be re-migrated.
- The `starlight-changelogs` plugin will use the `github` provider pointing to `RocketSurgeonsGuild/Conventions`; in local dev without a `GH_API_TOKEN`, it may degrade gracefully, which is acceptable.
- The Conventions repository uses the same GitHub Pages deployment mechanism as Indago; the pattern from `.github/workflows/` in the Indago repo can be adapted directly.
- The Conventions public API spans many packages; `xmldocmd` will be invoked once per package assembly to generate per-package reference docs under namespaced subdirectories within `docs/src/content/docs/api/`.
- Draft pages are not currently used in Conventions docs; `starlight-auto-drafts` requires no special content setup for this launch.
- The VS Code `starlight-links` extension recommendation (from Indago spec 002) SHOULD be added to `.vscode/extensions.json` as a quality-of-life improvement.
