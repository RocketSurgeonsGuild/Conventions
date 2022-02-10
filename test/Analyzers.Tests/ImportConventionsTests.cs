using System.Reflection;
using FluentAssertions;
using Xunit;
using static Rocket.Surgery.Conventions.Analyzers.Tests.GenerationHelpers;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class ImportConventionsTests
{
    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
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
            foreach (var convention in Dep2.Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Always()
    {
        var source = @"
";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
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
            foreach (var convention in Dep2.Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_Custom_Namespace()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions(Namespace = ""Test.My.Namespace"", ClassName = ""MyImports"")]
";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""Test.My.Namespace"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""MyImports"")]
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
            foreach (var convention in Dep2.Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Generate_Static_Assembly_Level_Method_FullName()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventionsAttribute]
";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
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
            foreach (var convention in Dep2.Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Generate_Static_Class_Member_Level_Method()
    {
        var source = @"
using Rocket.Surgery.Conventions;

namespace TestProject
{
    [ImportConventions]
    public partial class Program
    {
    }
}
";
        var expectedImports = @"using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
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
            foreach (var convention in Dep2.Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;
using TestProject.Conventions;

namespace TestProject
{
    public partial class Program
    {
        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider) => Imports.GetConventions(serviceProvider);
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            new[] { source },
            new[] { expectedImports, expected }
        ).ConfigureAwait(false);
    }


    [Fact]
    public async Task Should_Generate_Static_Class_Member_Level_Method_FullName()
    {
        var source = @"
using Rocket.Surgery.Conventions;

namespace TestProject
{
    [ImportConventionsAttribute]
    public partial class Program
    {
    }
}
";
        var expectedImports = @"using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
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
            foreach (var convention in Dep2.Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}";
        var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;
using TestProject.Conventions;

namespace TestProject
{
    public partial class Program
    {
        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider) => Imports.GetConventions(serviceProvider);
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            new[] { source },
            new[] { expectedImports, expected }
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Support_No_Exported_Convention_Assemblies()
    {
        var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
";
        var expectedImports = @"using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
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
        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            Enumerable.Empty<Assembly>(), new[] { source }, new[]
            {
                expectedImports
            }
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Support_Imports_And_Exports_In_The_Same_Assembly()
    {
        var source1 = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
";
        var source2 = @"
using Rocket.Surgery.Conventions;

namespace TestProject
{
    [ImportConventions]
    public partial class Program
    {
    }
}
";
        var expectedImports = @"using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
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
            foreach (var convention in TestProject.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}";
        var expected1 = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;
using TestProject.Conventions;

namespace TestProject
{
    public partial class Program
    {
        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider) => Imports.GetConventions(serviceProvider);
    }
}
";
        var expected2 = @"
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.ClassName"", ""Exports"")]
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
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.Contrib>(serviceProvider), HostType.Undefined);
        }
    }
}";

        var responses = await Generate<ConventionAttributesGenerator>(
            Array.Empty<Assembly>(),
            new[]
            {
                source1, source2
            }
        ).ConfigureAwait(false);

        foreach (var response in responses)
        {
            if (response.Contains("ExportedConventions", StringComparison.OrdinalIgnoreCase))
            {
                response.Should().Be(NormalizeToLf(expected2).Trim());
            }
            else if (response.Contains("class Program", StringComparison.OrdinalIgnoreCase))
            {
                response.Should().Be(NormalizeToLf(expected1).Trim());
            }
            else if (response.Contains("class Imports", StringComparison.OrdinalIgnoreCase))
            {
                response.Should().Be(NormalizeToLf(expectedImports).Trim());
            }
            else
            {
                throw new Exception("Could not find item to compare");
            }
        }
    }
}
