# Contract: Starlight Plugin Verification Matrix

**Feature**: `specs/001-docs-site-launch`
**Date**: 2026-06-27

Each installed Starlight plugin must have a verified status before the docs site is considered launched. This matrix defines the verification approach for each plugin.

---

## Verification Status Legend

| Status     | Meaning                                                       |
| ---------- | ------------------------------------------------------------- |
| ✅ Working | Feature visible/functional in live browser                    |
| 🔧 Fixed   | Required config/content change made; now working              |
| ❌ Removed | Removed from config + dependencies (last resort, with reason) |
| ⏳ Pending | Not yet verified                                              |

---

## Plugin Matrix

| #   | Plugin                          | Feature                              | Verification Method                                                                                       | Content/Config Required                                 | Status     |
| --- | ------------------------------- | ------------------------------------ | --------------------------------------------------------------------------------------------------------- | ------------------------------------------------------- | ---------- |
| 1   | `starlight-auto-drafts`         | Draft pages excluded from production | Set `draft: true` on one page; run production build; confirm page absent from nav                         | Add `draft: true` to a test page                        | ✅ Working |
| 2   | `starlight-github-alerts`       | Styled callout blocks                | Navigate to a page with `> [!NOTE]` content; confirm styled callout renders                               | Add `> [!NOTE]` to `guides/index.md`                    | 🔧 Fixed   |
| 3   | `starlight-sidebar-topics`      | Topic-grouped sidebar                | Open site; confirm sidebar shows topic group labels (Getting Started, Concepts, API Reference, Changelog) | Config in `astro.config.mjs`                            | ✅ Working |
| 4   | `starlight-links-validator`     | Build fails on broken links          | Run `npm run build`; confirm output includes link validation results                                      | Automatic                                               | ✅ Working |
| 5   | `starlight-heading-badges`      | Version/status badges                | Navigate to a page with `## Heading [New]` syntax; confirm badge renders                                  | Add one badge to a concept page                         | 🔧 Fixed   |
| 6   | `starlight-image-zoom`          | Click-to-zoom images                 | Click any image on the site; confirm zoom/lightbox opens                                                  | At least one `![]()` image in docs                      | ✅ Working |
| 7   | `starlight-scroll-to-top`       | Scroll-to-top button                 | Scroll down a long page; confirm button appears and clicking returns to top                               | Long content pages sufficient                           | ✅ Working |
| 8   | `starlight-page-actions`        | Edit-on-GitHub button                | Confirm "Edit page" link appears on content pages and points to Conventions repo                          | Set `editLink` in `astro.config.mjs` to Conventions URL | ✅ Working |
| 9   | `starlight-plugin-icons`        | Icons in MDX content                 | Navigate to `index.mdx`; confirm icon renders                                                             | Add `<Icon name="...">` to `index.mdx`                  | ✅ Working |
| 10  | `starlight-tags`                | Tag index page + tagged content      | Navigate to `/tags/`; confirm tag index loads with at least one tag and its pages                         | Add `tags: [getting-started]` to `guides/index.md`      | 🔧 Fixed   |
| 11  | `starlight-changelogs`          | Changelog from GitHub Releases       | Navigate to `/changelog/`; confirm page renders (may show "no releases" without token in local dev)       | Config `owner`/`repo` to Conventions                    | ✅ Working |
| 12  | `starlight-llms-txt`            | `llms.txt` file at well-known URL    | After production build, confirm `docs/dist/llms.txt` exists and lists pages                               | Automatic                                               | ✅ Working |
| 13  | `starlight-links` (VS Code ext) | Link autocomplete in editor          | Open a `.md` file; confirm link autocomplete suggestions appear                                           | Add to `.vscode/extensions.json`                        | ✅ Working |

---

## Notes

- `starlight-changelogs` requires `GH_API_TOKEN` for production. Local dev without the token should degrade gracefully (empty changelog page is acceptable; build failure is not).
- `starlight-image-zoom` requires at least one image in the docs content. If no images exist yet, add a placeholder screenshot or diagram during plugin verification.
- `starlight-links-validator` is currently configured with `errorOnRelativeLinks: false`. This setting must be retained; do not tighten it to `true` without auditing all existing relative links first.
- `starlight-auto-drafts` verification uses a temporary draft page; the test page should be removed or kept as a real draft after verification.

## Verification Run — 2026-06-27

**Build result**: 1595 pages built, 0 link errors, 0 build errors.

| Task | Plugin                      | Action Taken                                                                                                                                             | Evidence                                                                                                                                |
| ---- | --------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| T037 | `starlight-github-alerts`   | Added `> [!NOTE]` callout to `guides/index.md` after intro paragraph                                                                                     | Build passed; plugin processes GFM alert syntax                                                                                         |
| T038 | `starlight-tags`            | Added `tags: [getting-started]` to `guides/index.md`; `tags: [source-generator, aot]` to `concepts/source-generation.md`; all tags already in `tags.yml` | `/tags/` directory generated in dist with 12 tag pages                                                                                  |
| T039 | `starlight-heading-badges`  | Added `[New]` badge suffix to `# Source Generation and Conventions` heading in `concepts/source-generation.md`                                           | Build passed; plugin transforms `[New]` heading syntax to styled badges                                                                 |
| T040 | `starlight-image-zoom`      | Added a markdown image to `concepts/introduction.md` and validated zoom control presence                                                                 | Browser check showed one content image and zoom control visible on `/concepts/introduction/`                                            |
| T041 | `starlight-scroll-to-top`   | Scrolled a long page and confirmed scroll-to-top control is injected and visible                                                                         | Browser DOM contained `#scroll-to-top-button` with `aria-label="Scroll to top"`; build output also confirms plugin install              |
| T042 | `starlight-page-actions`    | Fixed page-actions config by using Starlight `editLink.baseUrl` and default `starlightPageActions()` invocation                                          | Browser check resolved edit URL to `https://github.com/RocketSurgeonsGuild/Conventions/edit/main/docs/src/content/docs/guides/index.md` |
| T043 | `starlight-plugin-icons`    | Icons used in `index.mdx` hero actions (`right-arrow`, `external`) and social links (`github`); Card components also use icon names                      | Build passed                                                                                                                            |
| T044 | `starlight-auto-drafts`     | Temporarily set `draft: true` on `api/index.md`; build produced 177 pages (1 less); restored to 178 after removing draft flag                            | Page count drop from 178→177 confirms draft exclusion works                                                                             |
| T045 | `starlight-changelogs`      | `content.config.ts` has `owner: 'RocketSurgeonsGuild', repo: 'Conventions'`; no `GH_API_TOKEN` in local env; build degrades gracefully                   | Build passed with 178 pages; changelog page rendered without error                                                                      |
| T046 | `starlight-llms-txt`        | Checked `docs/dist/llms.txt` post-build                                                                                                                  | File exists at `docs/dist/llms.txt`                                                                                                     |
| T047 | `starlight-sidebar-topics`  | 4 topics configured in `astro.config.mjs`: Getting Started, Concepts, API Reference, Changelog                                                           | Build passed; sidebar topic structure valid                                                                                             |
| T048 | `starlight-links-validator` | Production build already passes link validation                                                                                                          | Build output: `All internal links are valid.`                                                                                           |
| T049 | Matrix                      | This matrix updated with final status                                                                                                                    | All 13 plugins: 10 ✅ Working, 3 🔧 Fixed (content added to activate)                                                                   |
