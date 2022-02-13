using Xunit;
using static Rocket.Surgery.Conventions.Analyzers.Tests.GenerationHelpers;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class ExportedConventionsTests
{
    [Fact]
    public async Task Should_Pull_Through_A_Convention()
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
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.MethodName"", ""GetConventions"")]
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
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs"
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_Custom_Namespace()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: ExportConventions(Namespace = ""Source.Space"", ClassName = ""SourceClass"")]
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

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs"
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_No_Namespace()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: ExportConventions(Namespace = null)]
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

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs"
        ).ConfigureAwait(false);
    }


    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_Custom_MethodName()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: ExportConventions(MethodName = ""SourceMethod"")]
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

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs"
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_ExportAttribute()
    {
        var source = @"
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Conventions.Tests
{
    [ExportConventionAttribute]
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
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.MethodName"", ""GetConventions"")]
[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
[assembly: Convention(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
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
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs"
        ).ConfigureAwait(false);
    }

    [Theory]
    [InlineData(HostType.Live)]
    [InlineData(HostType.UnitTest)]
    public async Task Should_Support_HostType_Conventions(HostType hostType)
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    [{HostType}Convention]
    internal class Contrib : IConvention { }
}
".Replace("{HostType}", hostType.ToString(), StringComparison.OrdinalIgnoreCase);
        var expected = @"
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.ClassName"", ""Exports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.MethodName"", ""GetConventions"")]
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
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.Contrib>(serviceProvider), HostType.{HostType});
        }
    }
}
".Replace("{HostType}", hostType.ToString(), StringComparison.OrdinalIgnoreCase);
        ;

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs"
        ).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("AfterConventionAttribute", DependencyDirection.DependsOn)]
    [InlineData("DependsOnConventionAttribute", DependencyDirection.DependsOn)]
    [InlineData("BeforeConventionAttribute", DependencyDirection.DependentOf)]
    [InlineData("DependentOfConventionAttribute", DependencyDirection.DependentOf)]
    public async Task Should_Support_DependencyDirection_Conventions(string attributeName, DependencyDirection dependencyDirection)
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    [{AttributeName}(typeof(D))]
    [LiveConvention, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class Contrib : IConvention { }

    internal class D : IConvention { }
}
".Replace("{AttributeName}", attributeName, StringComparison.OrdinalIgnoreCase);
        var expected = @"
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.ClassName"", ""Exports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.MethodName"", ""GetConventions"")]
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
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.Contrib>(serviceProvider), HostType.Live).WithDependency(DependencyDirection.{DependencyDirection}, typeof(Rocket.Surgery.Conventions.Tests.D));
        }
    }
}
".Replace("{DependencyDirection}", dependencyDirection.ToString(), StringComparison.OrdinalIgnoreCase);
        ;

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            new[] { typeof(ExcludeFromCodeCoverageAttribute).Assembly },
            source,
            expected,
            "Exported_Conventions.cs"
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Pull_Through_All_Conventions()
    {
        var source1 = @"
using Rocket.Surgery.Conventions;

[assembly: ExportConventions(Namespace = ""Source.Space"")]
[assembly: Convention(typeof(Contrib1))]

internal class Contrib1 : IConvention { }
";
        var source2 = @"
using Rocket.Surgery.Conventions;

[assembly: Convention(typeof(Contrib3))]

[ExportConventionAttribute]
internal class Contrib2 : IConvention { }
internal class Contrib3 : IConvention { }
";
        var source3 = @"
using Rocket.Surgery.Conventions;

[assembly: Convention(typeof(Contrib4))]

internal class Contrib4 : IConvention { }
";
        var expectedImports = @"using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.ClassName"", ""Imports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Imports.MethodName"", ""GetConventions"")]";

        var expected = @"
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.Namespace"", ""Source.Space"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.ClassName"", ""Exports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.MethodName"", ""GetConventions"")]
[assembly: ExportedConventions(typeof(Contrib1), typeof(Contrib3), typeof(Contrib4), typeof(Contrib2))]
[assembly: Convention(typeof(Contrib2))]
namespace Source.Space
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
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Contrib1>(serviceProvider), HostType.Undefined);
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Contrib3>(serviceProvider), HostType.Undefined);
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Contrib4>(serviceProvider), HostType.Undefined);
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Contrib2>(serviceProvider), HostType.Undefined);
        }
    }
}";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            new[] { source1, source2, source3 },
            new[] { expected, expectedImports }
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Handle_Duplicate_Conventions()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]
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
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.MethodName"", ""GetConventions"")]
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
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs"
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Handle_Nested_Conventions()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(ParentContrib.Contrib))]
[assembly: Convention(typeof(ParentContrib.Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class ParentContrib {
        internal class Contrib : IConvention { }
    }
}
";
        var expected = @"
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.ClassName"", ""Exports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.MethodName"", ""GetConventions"")]
[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.ParentContrib.Contrib))]
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
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.ParentContrib.Contrib>(serviceProvider), HostType.Undefined);
        }
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs"
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Handle_Nested_Static_Conventions()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(ParentContrib.Contrib))]
[assembly: Convention(typeof(ParentContrib.Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal static class ParentContrib {
        internal class Contrib : IConvention { }
    }
}
";
        var expected = @"
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.Namespace"", ""TestProject.Conventions"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.ClassName"", ""Exports"")]
[assembly: System.Reflection.AssemblyMetadata(""Rocket.Surgery.ConventionConfigurationData.Exports.MethodName"", ""GetConventions"")]
[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.ParentContrib.Contrib))]
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
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.ParentContrib.Contrib>(serviceProvider), HostType.Undefined);
        }
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            await CreateDeps(),
            source,
            expected,
            "Exported_Conventions.cs"
        ).ConfigureAwait(false);
    }
}
