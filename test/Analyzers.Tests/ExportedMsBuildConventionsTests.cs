﻿using Xunit;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class ExportedMsBuildConventionsTests
{
    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_Custom_Namespace()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
";
        var expected = @"
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.Namespace"", ""Source.Space"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.ClassName"", ""SourceClass"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.MethodName"", ""GetConventions"")]
[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
namespace Source.Space
{
    /// <summary>
    /// The class defined for exporting conventions from this assembly
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static partial class SourceClass
    {
        /// <summary>
        /// The conventions exports from this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.Contrib>(serviceProvider), HostType.Undefined);
        }
    }
}
";

        await GenerationHelpers.AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await GenerationHelpers.CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs",
            new Dictionary<string, string?>
            {
                ["ExportConventionsNamespace"] = "Source.Space",
                ["ExportConventionsClassName"] = "SourceClass",
            }
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_No_Namespace()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
";
        var expected = @"
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.Namespace"", """")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.ClassName"", ""Exports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.MethodName"", ""GetConventions"")]
[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
/// <summary>
/// The class defined for exporting conventions from this assembly
/// </summary>
[System.Runtime.CompilerServices.CompilerGenerated]
public static partial class Exports
{
    /// <summary>
    /// The conventions exports from this assembly
    /// </summary>
    public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
    {
        yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.Contrib>(serviceProvider), HostType.Undefined);
    }
}
";

        await GenerationHelpers.AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await GenerationHelpers.CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs",
            new Dictionary<string, string?>
            {
                ["ExportConventionsNamespace"] = "",
            }
        ).ConfigureAwait(false);
    }


    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_Custom_MethodName()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
";
        var expected = @"
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.ClassName"", ""Exports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.MethodName"", ""SourceMethod"")]
[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
namespace TestProject.Conventions
{
    /// <summary>
    /// The class defined for exporting conventions from this assembly
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static partial class Exports
    {
        /// <summary>
        /// The conventions exports from this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> SourceMethod(IServiceProvider serviceProvider)
        {
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.Contrib>(serviceProvider), HostType.Undefined);
        }
    }
}
";

        await GenerationHelpers.AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await GenerationHelpers.CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs",
            new Dictionary<string, string?>
            {
                ["ExportConventionsMethodName"] = "SourceMethod",
            }
        ).ConfigureAwait(false);
    }
}
