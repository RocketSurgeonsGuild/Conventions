# Work Routing

How to decide who handles what.

## Routing Table

| Work Type | Route To | Examples |
|-----------|----------|----------|
| Site config, plugins, content pages | docs-engineer | astro.config.mjs, tags.yml, index.mdx, section landing pages, plugin verification |
| API reference generation, mise tasks | api-docs-engineer | generate-api.sh, docs:api/docs:build tasks, xmldocmd, dotnet build |
| CI/CD, GitHub Actions, GitHub Pages | devops-engineer | deploy-docs.yml, base path simulation, workflow triggers |
| Code review | ralph | Review PRs, check quality, suggest improvements |
| Scope & priorities | ralph | What to build next, trade-offs, decisions |
| Session logging | Scribe | Automatic — never needs routing |
| RAI review | Rai | Content safety, bias checks, credential detection, ethical review |

## Task Routing by Pattern

| Pattern | Route To |
|---------|----------|
| `astro.config.mjs`, `content.config.ts`, `tags.yml`, `index.mdx` | docs-engineer |
| `docs/src/content/docs/` (any content page) | docs-engineer |
| Plugin verification, representative content for plugins | docs-engineer |
| `.config/mise.toml` docs task definitions | api-docs-engineer |
| `docs/scripts/generate-api.sh`, `xmldocmd`, `add-api-frontmatter.mjs` | api-docs-engineer |
| `dotnet build`, API reference output under `docs/src/content/docs/api/` | api-docs-engineer |
| `.github/workflows/`, GitHub Pages deployment, CI base path | devops-engineer |

## Issue Routing

| Label | Action | Who |
|-------|--------|-----|
| `squad` | Triage: analyze issue, assign `squad:{member}` label | ralph |
| `squad:docs-engineer` | Pick up and complete the work | docs-engineer |
| `squad:api-docs-engineer` | Pick up and complete the work | api-docs-engineer |
| `squad:devops-engineer` | Pick up and complete the work | devops-engineer |
| `squad:ralph` | Pick up and complete the work | ralph |

### How Issue Assignment Works

1. When a GitHub issue gets the `squad` label, the **Lead** triages it — analyzing content, assigning the right `squad:{member}` label, and commenting with triage notes.
2. When a `squad:{member}` label is applied, that member picks up the issue in their next session.
3. Members can reassign by removing their label and adding another member's label.
4. The `squad` label is the "inbox" — untriaged issues waiting for Lead review.

## Rules

1. **Eager by default** — spawn all agents who could usefully start work, including anticipatory downstream work.
2. **Scribe always runs** after substantial work, always as `mode: "background"`. Never blocks.
3. **Quick facts → coordinator answers directly.** Don't spawn an agent for "what port does the server run on?"
4. **When two agents could handle it**, pick the one whose domain is the primary concern.
5. **"Team, ..." → fan-out.** Spawn all relevant agents in parallel as `mode: "background"`.
6. **Anticipate downstream work.** If a feature is being built, spawn the tester to write test cases from requirements simultaneously.
7. **Issue-labeled work** — when a `squad:{member}` label is applied to an issue, route to that member. The Lead handles all `squad` (base label) triage.
