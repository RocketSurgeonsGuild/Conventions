import type { Loader, LoaderContext } from 'astro/loaders';
import { docsLoader } from '@astrojs/starlight/loaders';
import { existsSync, readdirSync, readFileSync } from 'node:fs';
import { basename } from 'node:path';
import { fileURLToPath } from 'node:url';

/**
 * A single .NET project whose compiled XML documentation should be turned into API pages.
 */
export interface DotnetAssembly {
    /**
     * Project directory relative to the Astro project root (the `docs/` directory),
     * e.g. `../src/Indago`. Its `bin/<config>/<tfm>/<assembly>.xml` files are scanned.
     */
    projectDir: string;
    /** Assembly (and XML file) base name. Defaults to the last segment of {@link projectDir}. */
    assemblyName: string;
    /** Build configurations to search, in order of preference. The first one with any XML wins. */
    configurations?: string[];
}

/**
 * Options for {@link dotnetXmlApiLoader}.
 */
export interface DotnetXmlApiLoaderOptions {
    /** The assemblies to document. Every target framework of each assembly is parsed and merged. */
    assemblies: DotnetAssembly[];
    /** Namespaces to include. A member is kept when its full name starts with one of these + '.'. */
    includeNamespaces: string[];
    /** Slug prefix under which API pages are stored, e.g. `api`. */
    basePath?: string;
}

// ---------------------------------------------------------------------------
// XML doc parsing
// ---------------------------------------------------------------------------

type MemberKind = 'T' | 'M' | 'P' | 'F' | 'E';

interface RawMember {
    /** Full doc id without the kind prefix, e.g. `Indago.Abstractions.ITypeFilter.AssignableTo``1`. */
    id: string;
    kind: MemberKind;
    /** Inner XML of the `<member>` element. */
    xml: string;
}

interface TypeGroup {
    key: string; // namespace + '.' + simple name (arity stripped)
    namespace: string;
    simpleName: string;
    typeMembers: RawMember[]; // one per generic arity variant
    members: RawMember[]; // methods / properties / fields / events
}

/** The set of target frameworks each member id was found in, keyed by full doc id. */
type TfmIndex = Map<string, Set<string>>;

/** Splits the flat member list into `<member name>` id + inner xml pairs. */
function parseMembers(xml: string): RawMember[] {
    const out: RawMember[] = [];
    const re = /<member\s+name="([^"]+)">([\s\S]*?)<\/member>/g;
    let m: RegExpExecArray | null;
    while ((m = re.exec(xml)) !== null) {
        const [, name, body] = m;
        const kind = name[0] as MemberKind;
        if (name[1] !== ':') continue;
        out.push({ kind, id: name.slice(2), xml: body });
    }
    return out;
}

const stripArity = (s: string): string => s.replace(/`+\d+/g, '');

/** Everything before the last top-level `.` (nested types use `+`, generics use backticks — neither contains `.`). */
function splitDeclaringType(signature: string): { declaringType: string; memberName: string } {
    const withoutParams = signature.includes('(') ? signature.slice(0, signature.indexOf('(')) : signature;
    const lastDot = withoutParams.lastIndexOf('.');
    return {
        declaringType: withoutParams.slice(0, lastDot),
        memberName: withoutParams.slice(lastDot + 1),
    };
}

const namespaceOf = (fullType: string): string => {
    const noNested = fullType.split('+')[0];
    const lastDot = noNested.lastIndexOf('.');
    return lastDot > 0 ? noNested.slice(0, lastDot) : '';
};

const simpleNameOf = (fullType: string): string => {
    const cleaned = stripArity(fullType).replace(/\+/g, '.');
    return cleaned.slice(cleaned.lastIndexOf('.') + 1);
};

function isIncluded(id: string, namespaces: string[]): boolean {
    return namespaces.some(ns => id === ns || id.startsWith(`${ns}.`));
}

/** Groups members into per-type buckets, merging generic-arity variants of the same type. */
function groupByType(members: RawMember[], namespaces: string[]): TypeGroup[] {
    const groups = new Map<string, TypeGroup>();

    const ensure = (fullType: string): TypeGroup => {
        const key = stripArity(fullType).replace(/\+/g, '.');
        let g = groups.get(key);
        if (!g) {
            g = {
                key,
                namespace: namespaceOf(key),
                simpleName: simpleNameOf(key),
                typeMembers: [],
                members: [],
            };
            groups.set(key, g);
        }
        return g;
    };

    for (const member of members) {
        if (!isIncluded(member.id, namespaces)) continue;

        if (member.kind === 'T') {
            ensure(member.id).typeMembers.push(member);
        } else {
            const { declaringType } = splitDeclaringType(member.id);
            if (!isIncluded(declaringType, namespaces)) continue;
            ensure(declaringType).members.push(member);
        }
    }

    // Only surface types that were actually declared (have a T: member).
    return [...groups.values()].filter(g => g.typeMembers.length > 0).sort((a, b) => a.key.localeCompare(b.key));
}

// ---------------------------------------------------------------------------
// Target framework discovery, merging & tagging
// ---------------------------------------------------------------------------

interface TfmXml {
    tfm: string;
    path: string;
}

/** Finds every `<config>/<tfm>/<assembly>.xml` for the first configuration that has any. */
function discoverXmlFiles(binDirAbs: string, assemblyName: string, configurations: readonly string[]): TfmXml[] {
    for (const config of configurations) {
        const configDir = `${binDirAbs}/${config}`;
        if (!existsSync(configDir)) continue;

        const found: TfmXml[] = [];
        for (const entry of readdirSync(configDir, { withFileTypes: true })) {
            if (!entry.isDirectory()) continue;
            const path = `${configDir}/${entry.name}/${assemblyName}.xml`;
            if (existsSync(path)) found.push({ tfm: entry.name, path });
        }
        if (found.length > 0) return found;
    }
    return [];
}

const FAMILY_RANK: Record<string, number> = { netstandard: 0, netframework: 1, netcoreapp: 2, net: 3 };

/** Orders TFMs as netstandard → netcoreapp → net, ascending by version within each family. */
function compareTfms(a: string, b: string): number {
    const parse = (tfm: string): [number, number] => {
        const match = tfm.match(/^([a-z.]+?)(\d+(?:\.\d+)?)/i);
        const family = match ? match[1] : tfm;
        const version = match ? Number.parseFloat(match[2]) : 0;
        return [FAMILY_RANK[family] ?? 9, version];
    };
    const [fa, va] = parse(a);
    const [fb, vb] = parse(b);
    return fa - fb || va - vb || a.localeCompare(b);
}

const hasSummary = (xml: string): boolean => /<summary>[\s\S]*?\S[\s\S]*?<\/summary>/.test(xml);

/** Renders the target-framework badges for a set of TFMs (raw HTML consumed by the Markdown renderer). */
function tfmBadges(tfms: Iterable<string> | undefined): string {
    if (!tfms) return '';
    const sorted = [...tfms].sort(compareTfms);
    if (sorted.length === 0) return '';
    return sorted.map(tfm => `<span class="tfm-badge">${tfm}</span>`).join(' ');
}

// ---------------------------------------------------------------------------
// XML doc -> Markdown conversion
// ---------------------------------------------------------------------------

function decodeEntities(text: string): string {
    return text
        .replace(/&lt;/g, '<')
        .replace(/&gt;/g, '>')
        .replace(/&quot;/g, '"')
        .replace(/&#39;/g, "'")
        .replace(/&apos;/g, "'")
        .replace(/&amp;/g, '&');
}

/** Shortens a doc-comment type reference to a readable form, e.g. `System.Action{Indago...ITypeFilter}` -> `Action<ITypeFilter>`. */
function prettifyTypeRef(ref: string): string {
    // Drop the `T:` / `M:` etc. prefix if present.
    let s = ref.replace(/^[A-Z]:/, '').replace(/@$/, '');
    s = s.replace(/\{/g, '<').replace(/\}/g, '>');
    // Shorten each dotted identifier to its last segment; strip generic arity markers.
    return s.replace(/[A-Za-z_][A-Za-z0-9_.`]*/g, token => {
        const clean = stripArity(token);
        return clean.slice(clean.lastIndexOf('.') + 1);
    });
}

/** Escapes characters that Markdown would otherwise treat as raw HTML (e.g. generic `<T>` in headings). */
const escapeInline = (text: string): string => text.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');

/** Resolves the declaring-type key (namespace + name, arity-stripped) referenced by a `cref` or member id. */
function typeKeyFromRef(ref: string): string {
    const kind = /^[A-Z]:/.test(ref) ? ref[0] : 'T';
    const raw = ref.replace(/^[A-Z]:/, '');
    const typePart = kind === 'T' ? raw : splitDeclaringType(raw).declaringType;
    return stripArity(typePart).replace(/\+/g, '.');
}

/** A resolver from a normalized type key to a link href relative to the page being rendered. */
type TypeLinker = (typeKey: string) => string | undefined;

/** Builds an href from one page id to another, relative to the source page's URL (so it survives any base path). */
function relativeHref(fromId: string, toId: string): string {
    if (fromId === toId) return '';
    const from = fromId.split('/');
    const to = toId.split('/');
    let common = 0;
    const min = Math.min(from.length, to.length);
    while (common < min && from[common] === to[common]) common++;
    return '../'.repeat(from.length - common) + to.slice(common).join('/') + '/';
}

/** Renders a `<see cref>`/`<seealso cref>` target as a link to its page (when one exists) or inline code. */
function linkifyCref(cref: string, link: TypeLinker): string {
    const display = prettifyTypeRef(cref);
    const href = link(typeKeyFromRef(cref));
    return href ? `[${display}](${href})` : `\`${display}\``;
}

/** Renders a type expression (e.g. `System.Action{Indago...ITypeFilter}`) for a heading, linking any type that has a page. */
function linkifyTypeExpression(ref: string, link: TypeLinker): string {
    const s = ref
        .replace(/^[A-Z]:/, '')
        .replace(/@$/, '')
        .replace(/\{/g, '&lt;')
        .replace(/\}/g, '&gt;');
    return s.replace(/[A-Za-z_][A-Za-z0-9_.`]*/g, token => {
        const full = stripArity(token).replace(/\+/g, '.');
        const short = full.slice(full.lastIndexOf('.') + 1);
        const href = link(full);
        return href ? `[${short}](${href})` : escapeInline(short);
    });
}

/**
 * Converts an XML documentation fragment into Markdown, resolving `<inheritdoc>` against
 * the full member map and cross-linking `<see cref>` targets that have their own page.
 * Code blocks are protected from whitespace collapsing.
 */
function xmlToMarkdown(fragment: string, memberXmlById: Map<string, string>, link: TypeLinker, seen: Set<string> = new Set()): string {
    let s = fragment;

    // Resolve <inheritdoc cref="..."/> by inlining the referenced member's xml.
    s = s.replace(/<inheritdoc\s+cref="([^"]+)"\s*\/>/g, (_all, cref: string) => {
        const targetId = String(cref).replace(/^[A-Z]:/, '');
        if (seen.has(targetId)) return '';
        const target = memberXmlById.get(targetId);
        if (!target) return '';
        seen.add(targetId);
        return target;
    });

    // Protect <code> blocks.
    const codeBlocks: string[] = [];
    s = s.replace(/<code>([\s\S]*?)<\/code>/g, (_all, code: string) => {
        const idx = codeBlocks.push(decodeEntities(code).replace(/^\n+|\n+$/g, '')) - 1;
        return ` CODE${idx} `;
    });

    // Inline references.
    s = s.replace(/<see\s+langword="([^"]+)"\s*\/>/g, (_all, w: string) => `\`${w}\``);
    s = s.replace(/<see\s+href="([^"]+)">([\s\S]*?)<\/see>/g, (_all, href: string, txt: string) => `[${txt.trim()}](${href})`);
    s = s.replace(/<see\s+cref="([^"]+)"\s*\/>/g, (_all, cref: string) => linkifyCref(cref, link));
    s = s.replace(/<seealso\s+cref="([^"]+)"\s*\/>/g, (_all, cref: string) => linkifyCref(cref, link));
    s = s.replace(/<(?:paramref|typeparamref)\s+name="([^"]+)"\s*\/>/g, (_all, n: string) => `\`${n}\``);
    s = s.replace(/<c>([\s\S]*?)<\/c>/g, (_all, c: string) => `\`${decodeEntities(c).trim()}\``);

    // Lists -> bullet points.
    s = s.replace(/<list[^>]*>([\s\S]*?)<\/list>/g, (_all, inner: string) => {
        const items = [...String(inner).matchAll(/<item>([\s\S]*?)<\/item>/g)].map(it => {
            let content = it[1]
                .replace(/<term>([\s\S]*?)<\/term>/g, (_a, t: string) => `**${t.trim()}** `)
                .replace(/<description>([\s\S]*?)<\/description>/g, (_a, d: string) => d.trim());
            content = content
                .replace(/<[^>]+>/g, '')
                .replace(/\s+/g, ' ')
                .trim();
            return `- ${content}`;
        });
        return `\n\n${items.join('\n')}\n\n`;
    });

    // Paragraph breaks.
    s = s.replace(/<para>/g, '\n\n').replace(/<\/para>/g, '\n\n');

    // Drop any remaining tags, decode, collapse whitespace.
    s = s.replace(/<[^>]+>/g, '');
    s = decodeEntities(s);
    s = s
        .replace(/[ \t]+/g, ' ')
        .replace(/ *\n */g, '\n')
        .replace(/\n{3,}/g, '\n\n')
        .trim();

    // Restore code blocks.
    s = s.replace(/ CODE(\d+) /g, (_all, i: string) => `\n\n\`\`\`csharp\n${codeBlocks[Number(i)]}\n\`\`\`\n\n`);

    return s.trim();
}

/** Extracts the inner text of a single doc tag (e.g. `summary`, `returns`). */
function section(xml: string, tag: string): string | undefined {
    const m = xml.match(new RegExp(`<${tag}>([\\s\\S]*?)</${tag}>`));
    return m ? m[1] : undefined;
}

/** Extracts repeated named tags such as `<param name="x">...`. */
function namedSections(xml: string, tag: string): { name: string; body: string }[] {
    const out: { name: string; body: string }[] = [];
    const re = new RegExp(`<${tag}\\s+name="([^"]+)"\\s*>([\\s\\S]*?)</${tag}>`, 'g');
    let m: RegExpExecArray | null;
    while ((m = re.exec(xml)) !== null) out.push({ name: m[1], body: m[2] });
    return out;
}

/**
 * Renders a member heading as Markdown, e.g. `AddClasses(Action<ITypeFilter>, Boolean)`, escaping
 * generic angle brackets and linking any parameter type that has its own page.
 */
function renderMemberHeading(member: RawMember, link: TypeLinker): string {
    const { memberName } = splitDeclaringType(member.id);
    let name = escapeInline(stripArity(memberName).replace('#ctor', 'Constructor'));

    const genericArity = (memberName.match(/``(\d+)/) ?? [])[1];
    if (genericArity) name += escapeInline(`<${['T', 'U', 'V', 'W'].slice(0, Number(genericArity)).join(', ')}>`);

    if (member.kind === 'M') {
        // Parameter names come from `<param>` tags (declaration order); types come from the id.
        const paramNames = namedSections(member.xml, 'param').map(p => p.name);
        const params = memberParamTypes(member)
            .map((type, i) => {
                const linkedType = linkifyTypeExpression(type, link);
                return paramNames[i] ? `${linkedType} ${escapeInline(paramNames[i])}` : linkedType;
            })
            .join(', ');
        name += `(${params})`;
    }
    return name;
}

/** Splits a parameter type list on top-level commas (ignoring commas inside `{ }`). */
function splitTopLevel(list: string): string[] {
    const parts: string[] = [];
    let depth = 0;
    let current = '';
    for (const ch of list) {
        if (ch === '{') depth++;
        else if (ch === '}') depth--;
        if (ch === ',' && depth === 0) {
            parts.push(current);
            current = '';
        } else current = current + ch;
    }
    if (current) parts.push(current);
    return parts.map(p => p.trim());
}

/** The positional parameter type refs encoded in a member id, e.g. `[System.Action{...}, System.Boolean]`. */
function memberParamTypes(member: RawMember): string[] {
    if (!member.id.includes('(')) return [];
    const list = member.id.slice(member.id.indexOf('(') + 1, member.id.lastIndexOf(')'));
    return list ? splitTopLevel(list) : [];
}

const KIND_ORDER: MemberKind[] = ['M', 'P', 'F', 'E'];
const KIND_HEADINGS: Record<MemberKind, string> = { T: 'Types', M: 'Methods', P: 'Properties', F: 'Fields', E: 'Events' };

interface RenderContext {
    memberXmlById: Map<string, string>;
    memberTfms: TfmIndex;
    /** Whether the owning assembly targets more than one framework (controls badge output). */
    multiTfm: boolean;
    /** Resolves cross-references to other generated pages, relative to the page being rendered. */
    link: TypeLinker;
}

function renderTypePage(group: TypeGroup, ctx: RenderContext): { title: string; markdown: string } {
    const md: string[] = [];
    const convert = (xml?: string) => (xml ? xmlToMarkdown(xml, ctx.memberXmlById, ctx.link, new Set()) : '');

    // Prefer the lowest-arity variant for the type-level summary.
    const primary = [...group.typeMembers].sort((a, b) => a.id.length - b.id.length)[0];

    md.push(`\`namespace ${group.namespace}\``, '');

    if (ctx.multiTfm) {
        const typeTfms = new Set<string>();
        for (const t of group.typeMembers) for (const tfm of ctx.memberTfms.get(t.id) ?? []) typeTfms.add(tfm);
        const badges = tfmBadges(typeTfms);
        if (badges) md.push(badges, '');
    }

    const summary = convert(section(primary.xml, 'summary'));
    if (summary) md.push(summary, '');

    if (group.typeMembers.length > 1) {
        const forms = group.typeMembers.map(t => simpleNameOf(t.id) + `\`${(t.id.match(/`(\d+)/) ?? ['', ''])[1] || ''}\``).join(', ');
        md.push(`> Generic variants: ${forms}`, '');
    }

    const remarks = convert(section(primary.xml, 'remarks'));
    if (remarks) md.push('## Remarks', '', remarks, '');

    // Constructors first, then the rest by kind.
    const ctors = group.members.filter(m => m.kind === 'M' && m.id.includes('#ctor'));
    if (ctors.length > 0) {
        md.push('## Constructors', '');
        for (const c of ctors) md.push(...renderMember(c, convert, ctx));
    }

    for (const kind of KIND_ORDER) {
        const items = group.members.filter(m => m.kind === kind && !m.id.includes('#ctor'));
        if (items.length === 0) continue;
        md.push(`## ${KIND_HEADINGS[kind]}`, '');
        for (const item of items.sort((a, b) => a.id.localeCompare(b.id))) md.push(...renderMember(item, convert, ctx));
    }

    return {
        title: group.simpleName,
        markdown:
            md
                .join('\n')
                .replace(/\n{3,}/g, '\n\n')
                .trim() + '\n',
    };
}

function renderMember(member: RawMember, convert: (xml?: string) => string, ctx: RenderContext): string[] {
    const badges = ctx.multiTfm ? tfmBadges(ctx.memberTfms.get(member.id)) : '';
    const lines = [`### ${renderMemberHeading(member, ctx.link)}`, ''];
    if (badges) lines.push(badges, '');

    const summary = convert(section(member.xml, 'summary'));
    if (summary) lines.push(summary, '');

    const typeParams = namedSections(member.xml, 'typeparam').filter(t => convert(t.body));
    if (typeParams.length > 0) {
        lines.push('**Type parameters**', '');
        for (const tp of typeParams) lines.push(`- \`${tp.name}\` — ${convert(tp.body)}`);
        lines.push('');
    }

    // List every parameter from the signature (types from the id) with its name and description.
    const paramTypes = memberParamTypes(member);
    if (paramTypes.length > 0) {
        const paramDocs = namedSections(member.xml, 'param');
        const names = paramDocs.map(p => p.name);
        const descByName = new Map(paramDocs.map(p => [p.name, p.body]));
        lines.push('**Parameters**', '');
        paramTypes.forEach((type, i) => {
            const linkedType = linkifyTypeExpression(type, ctx.link);
            const name = names[i];
            const description = name ? convert(descByName.get(name)) : '';
            const label = name ? `\`${name}\` (${linkedType})` : `(${linkedType})`;
            lines.push(`- ${label}${description ? ` — ${description}` : ''}`);
        });
        lines.push('');
    }

    const returns = convert(section(member.xml, 'returns'));
    if (returns) lines.push('**Returns**', '', returns, '');

    for (const ex of namedSections(member.xml, 'exception')) {
        const desc = convert(ex.body);
        lines.push(`**Throws** \`${prettifyTypeRef(ex.name)}\`${desc ? ` — ${desc}` : ''}`, '');
    }

    const remarks = convert(section(member.xml, 'remarks'));
    if (remarks) lines.push(remarks, '');

    const example = convert(section(member.xml, 'example'));
    if (example) lines.push('**Example**', '', example, '');

    return lines;
}

/** Renders the `/api/` landing page: every generated type linked, grouped by namespace. */
function renderIndexPage(pages: { id: string; title: string; namespace: string }[], basePath: string): string {
    const md: string[] = ['API reference generated from the compiled .NET XML documentation.', ''];

    const byNamespace = new Map<string, { id: string; title: string }[]>();
    for (const page of pages) {
        (byNamespace.get(page.namespace) ?? byNamespace.set(page.namespace, []).get(page.namespace)!).push(page);
    }

    for (const namespace of [...byNamespace.keys()].sort((a, b) => a.localeCompare(b))) {
        md.push(`## ${namespace}`, '');
        for (const page of byNamespace.get(namespace)!.sort((a, b) => a.title.localeCompare(b.title))) {
            // Link relative to `/api/` so it works regardless of the site's base path.
            const href = `${page.id.slice(basePath.length + 1)}/`;
            md.push(`- [${page.title}](${href})`);
        }
        md.push('');
    }

    return md.join('\n').trim() + '\n';
}

// ---------------------------------------------------------------------------
// Loader
// ---------------------------------------------------------------------------

/**
 * A Starlight-compatible custom content loader that parses .NET XML documentation files and
 * emits one API-reference page per public type into the `docs` collection, alongside the
 * hand-written Markdown loaded by {@link docsLoader}. Every target framework of each assembly
 * is parsed and merged, and each type and member is tagged with the frameworks it exists in.
 */
export function dotnetXmlApiLoader(options: DotnetXmlApiLoaderOptions): Loader {
    const opts = {
        assemblies: options.assemblies ?? [],
        includeNamespaces: options.includeNamespaces ?? [],
        basePath: options.basePath ?? 'api',
    };

    const base = docsLoader();

    return {
        name: 'dotnet-xml-api-loader',
        async load(context: LoaderContext): Promise<void> {
            // 1. Load the hand-written Markdown docs first.
            await base.load(context);

            // Writes a generated Markdown page into the docs collection. Starlight's sidebar
            // `autogenerate` derives the route from `filePath` relative to the collection root
            // (`src/content/docs`), so a synthetic path is required.
            const writePage = async (id: string, title: string, markdown: string): Promise<void> => {
                const data = await context.parseData({ id, data: { title } });
                context.store.set({
                    id,
                    data,
                    body: markdown,
                    rendered: await context.renderMarkdown(markdown),
                    digest: context.generateDigest(markdown),
                    filePath: `src/content/docs/${id}.md`,
                });
            };

            // 2. Pass 1 — parse & merge every target framework of each assembly, assign a stable page
            //    id per type, and build a type-key → page-id index so cross-references can be linked.
            interface WorkItem {
                id: string;
                group: TypeGroup;
                memberXmlById: Map<string, string>;
                memberTfms: TfmIndex;
                multiTfm: boolean;
            }

            const usedSlugs = new Set<string>();
            const typeIndex = new Map<string, string>();
            const work: WorkItem[] = [];

            for (const assembly of opts.assemblies) {
                const assemblyName = assembly.assemblyName ?? basename(assembly.projectDir);
                const configurations = assembly.configurations ?? ['Release', 'Debug'];
                const binDirAbs = fileURLToPath(new URL(`${assembly.projectDir}/bin`, context.config.root));
                const xmlFiles = discoverXmlFiles(binDirAbs, assemblyName, configurations);

                if (xmlFiles.length === 0) {
                    context.logger.warn(
                        `No XML documentation found for '${assemblyName}' under ${binDirAbs}/{${configurations.join(',')}}/<tfm>/. ` +
                            `Its API reference pages will be skipped — build the project with GenerateDocumentationFile enabled.`
                    );
                    continue;
                }

                // Merge members across TFMs, tracking which frameworks each member appears in and
                // keeping the richest doc comment (some TFMs may leave a member undocumented).
                const memberTfms: TfmIndex = new Map();
                const bestMemberById = new Map<string, RawMember>();
                for (const { tfm, path } of xmlFiles) {
                    for (const member of parseMembers(readFileSync(path, 'utf8'))) {
                        (memberTfms.get(member.id) ?? memberTfms.set(member.id, new Set()).get(member.id)!).add(tfm);
                        const existing = bestMemberById.get(member.id);
                        if (!existing || (!hasSummary(existing.xml) && hasSummary(member.xml))) bestMemberById.set(member.id, member);
                    }
                }

                const allMembers = [...bestMemberById.values()];
                const memberXmlById = new Map(allMembers.map(m => [m.id, m.xml]));
                const groups = groupByType(allMembers, opts.includeNamespaces);
                const multiTfm = xmlFiles.length > 1;

                for (const group of groups) {
                    const nsSlug = group.namespace.toLowerCase().replace(/\./g, '/');
                    let id = `${opts.basePath}/${nsSlug}/${group.simpleName.toLowerCase()}`;
                    while (usedSlugs.has(id)) id = `${id}-`;
                    usedSlugs.add(id);
                    typeIndex.set(group.key, id);
                    work.push({ id, group, memberXmlById, memberTfms, multiTfm });
                }

                context.logger.info(
                    `Loaded ${groups.length} public type(s) for '${assemblyName}' from ${xmlFiles.length} target framework(s): ` +
                        `${xmlFiles
                            .map(x => x.tfm)
                            .sort(compareTfms)
                            .join(', ')}.`
                );
            }

            if (work.length === 0) {
                context.logger.warn('No API reference pages were generated from any configured assembly.');
                return;
            }

            // 3. Pass 2 — render each type page, resolving cross-references against the completed index.
            const pages: { id: string; title: string; namespace: string }[] = [];
            for (const item of work) {
                const link: TypeLinker = typeKey => {
                    const target = typeIndex.get(typeKey);
                    return target && target !== item.id ? relativeHref(item.id, target) : undefined;
                };
                const ctx: RenderContext = { memberXmlById: item.memberXmlById, memberTfms: item.memberTfms, multiTfm: item.multiTfm, link };
                const { title, markdown } = renderTypePage(item.group, ctx);
                await writePage(item.id, title, markdown);
                pages.push({ id: item.id, title, namespace: item.group.namespace });
            }

            // 4. Emit the `/api/` landing page listing every generated type, grouped by namespace.
            await writePage(opts.basePath, 'API Reference', renderIndexPage(pages, opts.basePath));
        },
    };
}
