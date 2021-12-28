using Sample.DependencyOne;
using Sample.DependencyThree;
using Sample.DependencyTwo;
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

[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
namespace TestProject.Conventions
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static partial class Exports
    {
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.Contrib>(serviceProvider), HostType.Undefined);
        }
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            new[] { typeof(Class1).Assembly, typeof(Class2).Assembly, typeof(Class3).Assembly },
            source,
            expected
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

[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
[assembly: Convention(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
namespace TestProject.Conventions
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static partial class Exports
    {
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.Contrib>(serviceProvider), HostType.Undefined);
        }
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            new[] { typeof(Class1).Assembly, typeof(Class2).Assembly, typeof(Class3).Assembly },
            source,
            expected
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

[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
namespace TestProject.Conventions
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static partial class Exports
    {
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.Contrib>(serviceProvider), HostType.{HostType});
        }
    }
}
".Replace("{HostType}", hostType.ToString(), StringComparison.OrdinalIgnoreCase);
        ;

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            new[] { typeof(Class1).Assembly, typeof(Class2).Assembly, typeof(Class3).Assembly },
            source,
            expected
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

[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
namespace TestProject.Conventions
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static partial class Exports
    {
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
            expected
        ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Should_Pull_Through_All_Conventions()
    {
        var source1 = @"
using Rocket.Surgery.Conventions;

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
        var expected = @"
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: ExportedConventions(typeof(Contrib1), typeof(Contrib3), typeof(Contrib4), typeof(Contrib2))]
[assembly: Convention(typeof(Contrib2))]
namespace TestProject.Conventions
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static partial class Exports
    {
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
            new[] { typeof(Class1).Assembly, typeof(Class2).Assembly, typeof(Class3).Assembly },
            new[] { source1, source2, source3 },
            new[] { expected }
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

[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
namespace TestProject.Conventions
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static partial class Exports
    {
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.Contrib>(serviceProvider), HostType.Undefined);
        }
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            new[] { typeof(Class1).Assembly, typeof(Class2).Assembly, typeof(Class3).Assembly },
            source,
            expected
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

[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.ParentContrib.Contrib))]
namespace TestProject.Conventions
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static partial class Exports
    {
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.ParentContrib.Contrib>(serviceProvider), HostType.Undefined);
        }
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            new[] { typeof(Class1).Assembly, typeof(Class2).Assembly, typeof(Class3).Assembly },
            source,
            expected
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

[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.ParentContrib.Contrib))]
namespace TestProject.Conventions
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static partial class Exports
    {
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.ParentContrib.Contrib>(serviceProvider), HostType.Undefined);
        }
    }
}
";

        await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
            new[] { typeof(Class1).Assembly, typeof(Class2).Assembly, typeof(Class3).Assembly },
            source,
            expected
        ).ConfigureAwait(false);
    }
}
