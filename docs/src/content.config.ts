import { defineCollection } from 'astro:content';
import { docsSchema } from '@astrojs/starlight/schema';
import { changelogsLoader } from 'starlight-changelogs/loader';
import { starlightTagsExtension } from 'starlight-tags/schema';
import { dotnetXmlApiLoader } from './loaders/dotnet-xml-api';

import { readdirSync, statSync } from 'fs';

const sourceFiles = readdirSync('../src')
    .filter(d => statSync(`../src/${d}`).isDirectory())
    .filter(z => !z.includes('.Analyzers'))
    .map(d => {
        return {
            projectDir: `../src/${d}`,
            assemblyName: `Rocket.Surgery.${d}`,
        };
    });

export const collections = {
    docs: defineCollection({
        // Custom loader: hand-written Markdown (via Starlight's docsLoader) plus API reference
        // pages parsed from each assembly's compiled XML documentation (all target frameworks).
        loader: dotnetXmlApiLoader({
            assemblies: sourceFiles,
            includeNamespaces: ['Rocket.Surgery', 'Microsoft.Extensions'],
            basePath: 'api',
        }),
        schema: docsSchema({ extend: starlightTagsExtension }),
    }),
    changelogs: defineCollection({
        loader: changelogsLoader([
            {
                provider: 'github',
                base: 'changelog',
                owner: 'RocketSurgeonsGuild',
                repo: 'Conventions',
                token: import.meta.env.GH_API_TOKEN,
            },
        ]),
    }),
};
