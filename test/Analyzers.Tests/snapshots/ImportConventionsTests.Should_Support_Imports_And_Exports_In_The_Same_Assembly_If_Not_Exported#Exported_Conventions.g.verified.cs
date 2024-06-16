//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Exported_Conventions.g.cs
#nullable enable
#pragma warning disable CS0105, CA1002, CA1034, CA1822, CS8602, CS8603, CS8618
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.Namespace", "TestProject.Conventions")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.ClassName", "Exports")]
[assembly: System.Reflection.AssemblyMetadata("Rocket.Surgery.ConventionConfigurationData.Exports.MethodName", "GetConventions")]
[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
[assembly: Convention(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
#pragma warning restore CS0105, CA1002, CA1034, CA1822, CS8602, CS8603, CS8618
#nullable restore
