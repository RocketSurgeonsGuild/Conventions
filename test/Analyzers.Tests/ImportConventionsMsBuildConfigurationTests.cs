using System.Reflection;
using Xunit;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class ImportConventionsMsBuildConfigurationTests
{
    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method()
    {
        var source = @"";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.MethodName"", ""GetConventions"")]
namespace TestProject.Conventions
{
    /// <summary>
    /// The class defined for importing conventions into this assembly
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal static partial class Imports
    {
        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            foreach (var convention in Dep1.Dep1Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";

        await GenerationHelpers.AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await GenerationHelpers.CreateDeps(),
            source,
            expected,
            properties: new Dictionary<string, string?>
            {
                ["ImportConventionsAssembly"] = "true",
            }
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Not_Generate_Static_Assembly_Level_Method_By_Default()
    {
        var source = @"";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.MethodName"", ""GetConventions"")]";

        await GenerationHelpers.AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await GenerationHelpers.CreateDeps(),
            source,
            expected,
            properties: new Dictionary<string, string?>
            {
                ["ImportConventionsAssembly"] = "false",
            }
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_Namespace()
    {
        var source = @"";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""Test.My.Namespace"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""MyImports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.MethodName"", ""GetConventions"")]
namespace Test.My.Namespace
{
    /// <summary>
    /// The class defined for importing conventions into this assembly
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal static partial class MyImports
    {
        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            foreach (var convention in Dep1.Dep1Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";

        await GenerationHelpers.AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await GenerationHelpers.CreateDeps(),
            source,
            expected,
            properties: new Dictionary<string, string?>
            {
                ["ImportConventionsNamespace"] = "Test.My.Namespace",
                ["ImportConventionsClassName"] = "MyImports",
            }
        ).ConfigureAwait(false);
    }


    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_No_Namespace()
    {
        var source = @"";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", """")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""MyImports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.MethodName"", ""GetConventions"")]
/// <summary>
/// The class defined for importing conventions into this assembly
/// </summary>
[System.Runtime.CompilerServices.CompilerGenerated]
internal static partial class MyImports
{
    /// <summary>
    /// The conventions imported into this assembly
    /// </summary>
    public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
    {
        foreach (var convention in Dep1.Dep1Exports.GetConventions(serviceProvider))
            yield return convention;
        foreach (var convention in Dep2Exports.GetConventions(serviceProvider))
            yield return convention;
        foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
            yield return convention;
    }
}
";

        await GenerationHelpers.AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await GenerationHelpers.CreateDeps(),
            source,
            expected,
            properties: new Dictionary<string, string?>
            {
                ["ImportConventionsNamespace"] = "",
                ["ImportConventionsClassName"] = "MyImports",
            }
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_MethodName()
    {
        var source = @"";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""Test.My.Namespace"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""MyImports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.MethodName"", ""ImportConventions"")]
namespace Test.My.Namespace
{
    /// <summary>
    /// The class defined for importing conventions into this assembly
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal static partial class MyImports
    {
        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> ImportConventions(IServiceProvider serviceProvider)
        {
            foreach (var convention in Dep1.Dep1Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";

        await GenerationHelpers.AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await GenerationHelpers.CreateDeps(),
            source,
            expected,
            properties: new Dictionary<string, string?>
            {
                ["ImportConventionsNamespace"] = "Test.My.Namespace",
                ["ImportConventionsClassName"] = "MyImports",
                ["ImportConventionsMethodName"] = "ImportConventions",
            }
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Use_Assembly_Configuration_If_Defined()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = ""Test.My.Namespace"", ClassName = ""MyImports"", MethodName = ""ImportConventions"")]
";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""Test.My.Namespace"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""MyImports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.MethodName"", ""ImportConventions"")]
namespace Test.My.Namespace
{
    /// <summary>
    /// The class defined for importing conventions into this assembly
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal static partial class MyImports
    {
        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> ImportConventions(IServiceProvider serviceProvider)
        {
            foreach (var convention in Dep1.Dep1Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";

        await GenerationHelpers.AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await GenerationHelpers.CreateDeps(),
            source,
            expected,
            properties: new Dictionary<string, string?>
            {
                ["ImportConventionsNamespace"] = "Test.Other.Namespace",
                ["ImportConventionsMethodName"] = "ImportsConventions",
            }
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_FullName()
    {
        var source = @"";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.MethodName"", ""GetConventions"")]
namespace TestProject.Conventions
{
    /// <summary>
    /// The class defined for importing conventions into this assembly
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal static partial class Imports
    {
        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            foreach (var convention in Dep1.Dep1Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";

        await GenerationHelpers.AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await GenerationHelpers.CreateDeps(),
            source,
            expected,
            properties: new Dictionary<string, string?>
            {
                ["ImportConventionsAssembly"] = "true",
            }
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Support_No_Exported_Convention_Assemblies()
    {
        var source = @"";
        var expectedImports = @"using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.MethodName"", ""GetConventions"")]
namespace TestProject.Conventions
{
    /// <summary>
    /// The class defined for importing conventions into this assembly
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal static partial class Imports
    {
        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield break;
        }
    }
}";
        await GenerationHelpers.AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            Enumerable.Empty<Assembly>(), new[] { source }, new[]
            {
                expectedImports
            },
            new Dictionary<string, string?>
            {
                ["ImportConventionsAssembly"] = "true"
            }
        ).ConfigureAwait(false);
    }
}
