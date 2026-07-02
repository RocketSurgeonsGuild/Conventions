---
title: Convention Context
description: The context object passed to every convention — how to create, configure, and query it.
---

# Convention Contexts

The convention context is the the result of collecting all the known conventions for a given set of assembly. How those assemblies are collected can diff
depending on on the convention context is created. Generally the convention context is created using the <xref:Rocket.Surgery.Clavus.ClavusContextBuilder>
however you can implement your own <xref:Rocket.Surgery.Clavus.IClavusContext> if you wish.

## Builder

A convention context is created from a <xref:Rocket.Surgery.Clavus.ClavusContextBuilder>. This class is used during the bootstrapping phase of your application.

You can add conventions manually, you can add them via attribute scanning, you can disable attribute scanning if you wish as well.

> [!NOTE]
> The assemblies used during scanning can be added by using an AppDomain, DependencyContext, or List of assemblies.

## Creating the context

A context can be created from a <xref:Rocket.Surgery.Clavus.ClavusContextBuilder> by using [ClavusContext.From](xref:Rocket.Surgery.Clavus.ClavusContext#Rocket_Surgery_Conventions_ClavusContext_From_Rocket_Surgery_Conventions_ClavusContextBuilder_).

## Using the context

Once the context is created you can use the context to find out all sorts of information.

Useful properties:

- [`AssemblyProvider`](xref:Rocket.Surgery.Clavus.IClavusContext#Rocket_Surgery_Conventions_ClavusContext_AssemblyProvider) - The type provider can be used to get a list of assemblies
- [`Logger`](xref:Rocket.Surgery.Clavus.IClavusContext#Rocket_Surgery_Conventions_ClavusContext_Logger) - This is a diagnostic logger that can be used for logging details. If a logger is provided to the builder it will be used here.
- [`Properties`](xref:Rocket.Surgery.Clavus.IClavusContext#Rocket_Surgery_Conventions_ClavusContext_Properties) - Contains all the properties provided to the builder. This implements `IServiceProvider` and can be used with `ActivatorExtensions

Useful methods / extension methods:

- [`Get<T>([string name])`](xref:Rocket.Surgery.Clavus.ClavusContextExtensions#Rocket_Surgery_Conventions_ClavusContextExtensions_Get__1_Rocket_Surgery_Conventions_IClavusContext_) - Get a given type from the `Properties` dictionary.
- [`GetHostType()`](xref:Rocket.Surgery.Clavus.ClavusContextExtensions#Rocket_Surgery_Conventions_ClavusContextExtensions_GetHostType_Rocket_Surgery_Conventions_IClavusContext_) - Get's the given host type, as defined in the builder.
- [`IsUnitTestHost()`](xref:Rocket.Surgery.Clavus.ClavusContextExtensions#Rocket_Surgery_Conventions_ClavusContextExtensions_IsUnitTestHost_Rocket_Surgery_Conventions_IClavusContext_) - Tests if the builder was setup for unit testing
    - This is handy to ensure different behavior during [unit tests](./unit-tests.md).
