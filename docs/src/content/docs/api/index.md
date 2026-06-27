---
title: API Reference
description: Auto-generated API reference for all public Conventions packages.
---

import { CardGrid, LinkCard } from '@astrojs/starlight/components';

API reference documentation is auto-generated from XML doc comments in the source code. Run `mise run docs:api` to generate the reference pages.

:::note
The API reference pages are generated at build time. If you don't see package links below, run `mise run docs:api` first.
:::

<CardGrid>
	<LinkCard title="Conventions" href="/api/conventions/" description="Core conventions and runtime wiring contracts." />
	<LinkCard title="Abstractions" href="/api/abstractions/" description="Public abstraction interfaces such as IConvention and IConventionContext." />
	<LinkCard title="CommandLine" href="/api/commandline/" description="Command-line specific conventions and helpers." />
	<LinkCard title="Hosting" href="/api/hosting/" description="Generic host integration APIs." />
	<LinkCard title="Web.Hosting" href="/api/web-hosting/" description="ASP.NET Core hosting integrations." />
	<LinkCard title="WebAssembly.Hosting" href="/api/webassembly-hosting/" description="Blazor WebAssembly hosting integrations." />
	<LinkCard title="Aspire.Hosting" href="/api/aspire-hosting/" description=".NET Aspire hosting conventions." />
	<LinkCard title="Aspire.Hosting.Testing" href="/api/aspire-hosting-testing/" description="Aspire testing integration APIs." />
	<LinkCard title="Autofac" href="/api/autofac/" description="Autofac container integration APIs." />
	<LinkCard title="DryIoc" href="/api/dryioc/" description="DryIoc container integration APIs." />
	<LinkCard title="Serilog" href="/api/serilog/" description="Serilog logging convention APIs." />
	<LinkCard title="Configuration.Json" href="/api/configuration-json/" description="JSON configuration convention APIs." />
	<LinkCard title="Configuration.Yaml" href="/api/configuration-yaml/" description="YAML configuration convention APIs." />
	<LinkCard title="Diagnostics" href="/api/diagnostics/" description="Diagnostics and instrumentation helpers." />
</CardGrid>
