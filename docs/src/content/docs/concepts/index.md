---
title: Concepts
description: Core concepts behind Conventions — how convention-driven wiring works in .NET.
---

import { CardGrid, LinkCard } from '@astrojs/starlight/components';

Conventions replaces runtime reflection-based application wiring with compile-time source generation. These pages explain the core ideas behind the library.

<CardGrid>
  <LinkCard title="Introduction" href="/concepts/introduction/" description="What Conventions is and why it exists." />
  <LinkCard title="Defining Conventions" href="/concepts/defining-conventions/" description="How to create and register your own conventions." />
  <LinkCard title="Convention Context" href="/concepts/convention-context/" description="The context object passed to every convention." />
  <LinkCard title="Source Generation" href="/concepts/source-generation/" description="How the Roslyn generator resolves conventions at build time." />
  <LinkCard title="Unit Tests" href="/concepts/unit-tests/" description="Testing conventions in isolation." />
</CardGrid>
