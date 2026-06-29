# Contract: Site Identity & Configuration

**Feature**: `specs/001-docs-site-launch`
**Date**: 2026-06-27

This contract defines every Conventions-specific value that must replace the current Indago references in the docs site configuration.

---

## `astro.config.mjs` Changes

```js
// Base path
const base = process.env.GITHUB_ACTIONS ? '/Conventions' : '';
//                                         ^^^^^^^^^^ was '/Indago'

// Starlight config
starlight({
    title: 'Conventions',
    //      ^^^^^^^^^^^ was 'Indago'

    description: 'Convention-driven application wiring for .NET. AOT-safe, zero runtime reflection.',
    // was: 'Compile-time assembly/type-scanning...'

    social: [
        {
            icon: 'github',
            label: 'GitHub',
            href: 'https://github.com/RocketSurgeonsGuild/Conventions',
            //                                              ^^^^^^^^^^^ was 'Indago'
        },
    ],

    plugins: [
        // ... (other plugins unchanged)
        starlightPageActions({
            editLink: 'https://github.com/RocketSurgeonsGuild/Conventions/edit/main/docs/src/content/docs/',
            //                                                 ^^^^^^^^^^^ was 'Indago'
        }),
        // ...
    ],
});
```

## Sidebar Topics (full replacement)

```js
starlightSidebarTopics(
    [
        {
            label: 'Getting Started',
            link: '/guide/',
            icon: 'open-book',
            items: [{ autogenerate: { directory: 'guides' } }],
        },
        {
            label: 'Concepts',
            link: '/concepts/',
            icon: 'information',
            items: [{ autogenerate: { directory: 'concepts' } }],
        },
        {
            label: 'API Reference',
            link: '/api/',
            icon: 'seti:brackets',
            items: [{ autogenerate: { directory: 'api' } }],
        },
        {
            label: 'Changelog',
            link: '/changelog/',
            icon: 'list-format',
        },
    ],
    {
        exclude: ['/tags/**', '/changelog/**'],
    }
);
```

**Removed topics** (not in Conventions content structure): `Reference` (`/reference/`), `Architecture` (`/architecture/`).

---

## `docs/src/content.config.ts` Changes

```ts
changelogs: defineCollection({
    loader: changelogsLoader([
        {
            provider: 'github',
            base: 'changelog',
            owner: 'RocketSurgeonsGuild',
            repo: 'Conventions', // was 'Indago'
            token: import.meta.env.GH_API_TOKEN,
        },
    ]),
});
```

---

## `docs/src/content/docs/index.mdx` Changes

```mdx
---
title: Conventions — Convention-Driven Wiring for .NET
description: Convention-driven application wiring for .NET. AOT-safe, zero runtime reflection. Replaces assembly scanning with compile-time source generation.
template: splash
hero:
    title: Conventions
    tagline: Convention-driven application wiring for .NET. AOT-safe, zero runtime reflection.
    actions:
        - text: Get Started
          link: /guide/
          icon: right-arrow
          variant: primary
        - text: View on GitHub
          link: https://github.com/RocketSurgeonsGuild/Conventions
          icon: external
          variant: minimal
    banner:
        content: |
            Conventions supports .NET 8 and .NET 10.
            <a href="https://github.com/RocketSurgeonsGuild/Conventions/releases">See releases →</a>
---
```

---

## `docs/package.json` Change

```json
{
    "name": "conventions-docs",
    "...": "..."
}
```

(Minor — only the `name` field changes from `"indago-docs"` to `"conventions-docs"`.)

---

## Section Landing Pages to Create

### `docs/src/content/docs/concepts/index.md`

```markdown
---
title: Concepts
description: Core concepts behind the Conventions library — conventions, context, source generation, and unit testing patterns.
---

# Concepts

Explore the foundational concepts behind Rocket Surgeons Guild Conventions.

- [Introduction](./introduction) — What are Conventions and why do they exist?
- [Defining Conventions](./defining-conventions) — How to author and register a convention.
- [Convention Context](./convention-context) — The shared context passed to conventions at startup.
- [Source Generation](./source-generation) — How the Roslyn generator resolves conventions at build time.
- [Unit Tests](./unit-tests) — Patterns for testing conventions in isolation.
```

### `docs/src/content/docs/api/index.md`

Created after `mise run docs:api` generates the package subdirectories. Template:

```markdown
---
title: API Reference
description: Auto-generated API reference for all Rocket Surgeons Guild Conventions packages.
---

# API Reference

Complete API reference generated from the Conventions assemblies and their XML documentation.

## Packages

- [Conventions](./abstractions/) — Core convention types and interfaces
- [Conventions.Abstractions](./abstractions/) — Public abstractions and contracts
- [Conventions.Analyzers](./analyzers/) — Roslyn source generator
- [Conventions.Autofac](./autofac/) — Autofac container integration
- ... (remaining packages)
```

---

## `.vscode/extensions.json` Addition

```json
{
    "recommendations": ["...(existing)", "HiDeoo.starlight-links"]
}
```
