//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Exported_Conventions.g.cs
#pragma warning disable CA1822
#pragma warning disable CS8618
#pragma warning disable CS8603
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.Namespace", "TestProject.Conventions")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.ClassName", "Exports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.MethodName", "GetConventions")]
[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
[assembly: Convention(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]