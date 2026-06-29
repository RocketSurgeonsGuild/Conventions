#!/usr/bin/env node
import fs from 'node:fs';
import path from 'node:path';

const [xmlPath, outDir] = process.argv.slice(2);

if (!xmlPath || !outDir) {
    console.error('Usage: node generate-api-from-xml.mjs <xml-path> <output-dir>');
    process.exit(1);
}

const xml = fs.readFileSync(xmlPath, 'utf8');
fs.mkdirSync(outDir, { recursive: true });

function decode(text) {
    return text
        .replace(/&lt;/g, '<')
        .replace(/&gt;/g, '>')
        .replace(/&amp;/g, '&')
        .replace(/&quot;/g, '"')
        .replace(/&#39;/g, "'")
        .replace(/\s+/g, ' ')
        .trim();
}

function tag(block, name) {
    const match = block.match(new RegExp(`<${name}>([\\s\\S]*?)<\\/${name}>`, 'i'));
    return match ? decode(match[1]) : '';
}

function paramTags(block) {
    const result = [];
    const re = /<param\s+name="([^"]+)">([\s\S]*?)<\/param>/gi;
    let match = re.exec(block);
    while (match) {
        result.push({ name: match[1], description: decode(match[2]) });
        match = re.exec(block);
    }
    return result;
}

const types = new Map();
const methodsByType = new Map();

const memberRe = /<member\s+name="([^"]+)">([\s\S]*?)<\/member>/gi;
let memberMatch = memberRe.exec(xml);
while (memberMatch) {
    const id = memberMatch[1];
    const block = memberMatch[2];

    if (id.startsWith('T:')) {
        const fullType = id.slice(2);
        types.set(fullType, {
            fullType,
            summary: tag(block, 'summary'),
        });
    }

    if (id.startsWith('M:')) {
        const methodSig = id.slice(2);
        const sigNoParams = methodSig.includes('(') ? methodSig.slice(0, methodSig.indexOf('(')) : methodSig;
        const lastDot = sigNoParams.lastIndexOf('.');
        if (lastDot > 0) {
            const owner = sigNoParams.slice(0, lastDot);
            const methodName = sigNoParams.slice(lastDot + 1).replace('#ctor', 'ctor');
            const methods = methodsByType.get(owner) ?? [];
            methods.push({
                signature: methodSig,
                name: methodName,
                summary: tag(block, 'summary'),
                params: paramTags(block),
            });
            methodsByType.set(owner, methods);
        }
    }

    memberMatch = memberRe.exec(xml);
}

for (const [fullType, typeInfo] of types) {
    const simpleName = fullType.split('.').pop().replace(/\+/g, '.');
    const methods = methodsByType.get(fullType) ?? [];
    const filePath = path.join(outDir, `${simpleName}.md`);

    const lines = ['---', `title: ${JSON.stringify(simpleName)}`, '---', '', `# ${simpleName}`, ''];

    if (typeInfo.summary) {
        lines.push(typeInfo.summary, '');
    }

    if (methods.length > 0) {
        lines.push('## Methods', '');

        for (const method of methods) {
            lines.push(`### ${method.name}`, '');
            if (method.summary) {
                lines.push(method.summary, '');
            }

            if (method.params.length > 0) {
                lines.push('#### Parameters', '');
                for (const p of method.params) {
                    lines.push(`- ${p.name}: ${p.description || 'No description.'}`);
                }
                lines.push('');
            }
        }
    }

    fs.writeFileSync(filePath, lines.join('\n'));
}

console.log(`Generated ${types.size} type pages from XML: ${path.basename(xmlPath)}`);
