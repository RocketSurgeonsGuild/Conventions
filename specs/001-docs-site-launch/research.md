# Research: Conventions Documentation Site Launch

**Feature**: `specs/001-docs-site-launch/spec.md`
**Date**: 2026-06-27
**Reference Implementation**: Indago `specs/002-starlight-docs-migration` + `specs/003-docs-site-fixes`

---

## Decision 1: Tech Stack (already decided — no research needed)

**Decision**: Astro Starlight with the 13-plugin set already installed in `docs/package.json`.

**Rationale**: Stack is inherited from Indago where it was fully evaluated (spec 002). The `docs/` directory already has `package.json`, `astro.config.mjs`, `tsconfig.json`, and `content.config.ts` in place. No evaluation needed.

**Alternatives considered**: VitePress (replaced; `mise.toml` still references it), DocFX (HTML-only, incompatible with Starlight).

---

## Decision 2: API Reference Tool (already decided — confirmed)

**Decision**: `xmldocmd` 2.9.0 via mise.

**Rationale**: Already installed in `.config/mise.toml` as `"dotnet:xmldocmd" = "2.9.0"`. The `docs/scripts/add-api-frontmatter.mjs` script for post-processing is already written. Proven in Indago.

**Invocation pattern** (from Indago, adapted for Conventions multi-package):

```sh
# Per package — example for Conventions.Abstractions
dotnet publish src/Conventions.Abstractions/Conventions.Abstractions.csproj \
  -c Release -f net10.0 -o /tmp/conventions-publish --nologo

xmldocmd /tmp/conventions-publish/Rocket.Surgery.Conventions.Abstractions.dll \
  docs/src/content/docs/api/abstractions/ \
  --namespace Rocket.Surgery.Conventions \
  --source https://github.com/RocketSurgeonsGuild/Conventions/blob/main/src/Conventions.Abstractions/

node docs/scripts/add-api-frontmatter.mjs
```

**Packages to generate API docs for** (all 16 in `src/`; all have `<GenerateDocumentationFile>true`):

| Folder                           | DLL Name                                                | Output Subdir                 |
| -------------------------------- | ------------------------------------------------------- | ----------------------------- |
| `Conventions`                    | `Rocket.Surgery.Conventions.dll`                        | `api/conventions/`            |
| `Conventions.Abstractions`       | `Rocket.Surgery.Conventions.Abstractions.dll`           | `api/abstractions/`           |
| `Conventions.Analyzers`          | `Rocket.Surgery.Conventions.Analyzers.dll`              | `api/analyzers/`              |
| `Conventions.Autofac`            | `Rocket.Surgery.Conventions.Autofac.dll`                | `api/autofac/`                |
| `Conventions.Configuration.Json` | `Rocket.Surgery.Conventions.Configuration.Json.dll`     | `api/configuration-json/`     |
| `Conventions.Configuration.Yaml` | `Rocket.Surgery.Conventions.Configuration.Yaml.dll`     | `api/configuration-yaml/`     |
| `Conventions.Diagnostics`        | `Rocket.Surgery.Conventions.Diagnostics.dll`            | `api/diagnostics/`            |
| `Conventions.DryIoc`             | `Rocket.Surgery.Conventions.DryIoc.dll`                 | `api/dryioc/`                 |
| `Conventions.Serilog`            | `Rocket.Surgery.Conventions.Serilog.dll`                | `api/serilog/`                |
| `CommandLine`                    | `Rocket.Surgery.Conventions.CommandLine.dll`            | `api/commandline/`            |
| `Hosting`                        | `Rocket.Surgery.Conventions.Hosting.dll`                | `api/hosting/`                |
| `Web.Hosting`                    | `Rocket.Surgery.Conventions.WebAssembly.Hosting.dll`    | `api/web-hosting/`            |
| `WebAssembly.Hosting`            | `Rocket.Surgery.Conventions.WebAssembly.Hosting.dll`    | `api/webassembly-hosting/`    |
| `Aspire.Hosting`                 | `Rocket.Surgery.Conventions.Aspire.Hosting.dll`         | `api/aspire-hosting/`         |
| `Aspire.Hosting.Testing`         | `Rocket.Surgery.Conventions.Aspire.Hosting.Testing.dll` | `api/aspire-hosting-testing/` |

> **Note**: The `Conventions.Analyzers.roslyn4.8` project is a Roslyn-version variant for analyzer compatibility testing, not a separate NuGet package. It shares the same output as `Conventions.Analyzers`. Skip it in API generation.

**Simplified approach**: Rather than per-package publish+xmldocmd invocations, a single `dotnet build -c Release` followed by scanning `src/*/bin/Release/net10.0/*.dll` may be more practical. The `docs:api` mise task should use a shell glob or a dedicated script to handle all packages. See contracts for the exact task structure.

---

## Decision 3: Mise Task Names (adapted from Indago pattern)

**Decision**: Match the Indago task names exactly for consistency across RSG repos.

| Task           | Command                                         | Description                                                          |
| -------------- | ----------------------------------------------- | -------------------------------------------------------------------- |
| `docs`         | `cd docs && npm run dev`                        | Start Astro dev server                                               |
| `docs:build`   | `mise run docs:api && cd docs && npm run build` | Generate API reference then build                                    |
| `docs:api`     | _(see contracts/mise-tasks.md)_                 | Build Conventions, run xmldocmd for all packages, inject frontmatter |
| `docs:preview` | `cd docs && npm run preview`                    | Preview production build                                             |

**Current state**: `docs = { run = "vitepress dev docs" }` and `docs-preview = { run = "vitepress preview docs" }` need replacement. `docs:api` and `docs:build` are new additions.

---

## Decision 4: Sidebar Topic Structure

**Decision**: 4 topics matching the existing content layout:

```js
starlightSidebarTopics(
    [
        { label: 'Getting Started', link: '/guide/', icon: 'open-book', items: [{ autogenerate: { directory: 'guides' } }] },
        { label: 'Concepts', link: '/concepts/', icon: 'information', items: [{ autogenerate: { directory: 'concepts' } }] },
        { label: 'API Reference', link: '/api/', icon: 'seti:brackets', items: [{ autogenerate: { directory: 'api' } }] },
        { label: 'Changelog', link: '/changelog/', icon: 'list-format' },
    ],
    { exclude: ['/tags/**', '/changelog/**'] }
);
```

**Rationale**: The Conventions content has guides and concepts — no `reference/` or `architecture/` directories exist. Matching the actual content avoids 404 section roots. The Indago astro.config.mjs currently in `docs/` has 5 topics including `reference/` and `architecture/` — these need to be removed.

**Missing landing pages**: `concepts/index.md` and `api/index.md` need to be created. `guides/index.md` already exists.

---

## Decision 5: CI Workflow Strategy

**Decision**: Replace the current `publish-docs.yml` (which downloads a CI artifact) with a dedicated `deploy-docs.yml` that builds and deploys directly, matching the Indago pattern.

**Rationale**: The existing `publish-docs.yml` uses `dawidd6/action-download-artifact` to pull a `docs` artifact uploaded by the main CI workflow. This pattern requires coordination with the CI workflow and adds complexity. The Indago pattern uses a self-contained deploy workflow that does its own build + deploy in one job. This is simpler and more direct.

**New workflow pattern** (from Indago `deploy-docs.yml`):

- Trigger: `push` to main when `docs/**` changes + `workflow_dispatch`
- `build` job: checkout → setup .NET → setup Node → build dotnet → run `docs:api` → `npm ci && npm run build` → upload Pages artifact
- `deploy` job: `actions/deploy-pages@v4`
- Base path: `GITHUB_ACTIONS ? '/Conventions' : ''`

---

## Decision 6: Base Path

**Decision**: `/Conventions` in GitHub Actions; `/` locally.

**Current state**: `astro.config.mjs` has `const base = process.env.GITHUB_ACTIONS ? '/Indago' : ''` — change `'/Indago'` to `'/Conventions'`.

---

## Decision 7: Changelog Configuration

**Decision**: Update `content.config.ts` to point `starlight-changelogs` at `RocketSurgeonsGuild/Conventions`.

**Current state**: `content.config.ts` has `repo: 'Indago'` — change to `repo: 'Conventions'`.

---

## Decision 8: Tags

**Decision**: Update `docs/tags.yml` for Conventions-specific topics.

**Current tags** (from Indago, need Conventions adaptation):

| Tag                | Keep? | Notes                           |
| ------------------ | ----- | ------------------------------- |
| `aot`              | ✅    | Conventions supports AOT        |
| `source-generator` | ✅    | Core Conventions feature        |
| `getting-started`  | ✅    | Retain                          |
| `api`              | ✅    | Retain                          |
| `di`               | ✅    | Core use case                   |
| `architecture`     | ✅    | Retain for architecture content |

Additional Conventions-specific tags to add:

| Tag             | Label         | Description                                      |
| --------------- | ------------- | ------------------------------------------------ |
| `hosting`       | Hosting       | Generic Host and ASP.NET Core hosting extensions |
| `configuration` | Configuration | Configuration convention patterns                |
| `logging`       | Logging       | Serilog and logging conventions                  |
| `autofac`       | Autofac       | Autofac container integration                    |
| `dryioc`        | DryIoc        | DryIoc container integration                     |
| `aspire`        | Aspire        | .NET Aspire integration                          |

---

## Decision 9: Plugin Activation Content

**Decision**: Add representative content to activate all plugins that require frontmatter or specific syntax.

| Plugin                      | Requires                    | Content to Add                                       |
| --------------------------- | --------------------------- | ---------------------------------------------------- |
| `starlight-tags`            | `tags:` in frontmatter      | Add tags to `index.mdx` and concept pages            |
| `starlight-heading-badges`  | `## Heading [badge]` syntax | Add one badge to a reference or concept page         |
| `starlight-github-alerts`   | `> [!NOTE]` syntax          | Add an alert to `guides/index.md`                    |
| `starlight-auto-drafts`     | `draft: true`               | No representative content needed (exclude from prod) |
| `starlight-changelogs`      | GitHub token                | Token needed in CI; local dev degrades gracefully    |
| `starlight-image-zoom`      | Any `![]()` image           | Add a diagram or screenshot to one page              |
| `starlight-scroll-to-top`   | Long page                   | Long content pages (concept pages are sufficient)    |
| `starlight-page-actions`    | `editLink` config           | Set in `astro.config.mjs`                            |
| `starlight-plugin-icons`    | `<Icon>` in MDX             | Can add to `index.mdx`                               |
| `starlight-llms-txt`        | Automatic                   | No content needed                                    |
| `starlight-links-validator` | Automatic                   | No content needed                                    |
| `starlight-sidebar-topics`  | Config                      | Set in `astro.config.mjs`                            |
| `starlight-image-zoom`      | Images                      | At least one image needed                            |

---

## Unresolved Items

None. All NEEDS CLARIFICATION items from spec have been resolved. All tech choices were pre-made based on the Indago reference implementation.
