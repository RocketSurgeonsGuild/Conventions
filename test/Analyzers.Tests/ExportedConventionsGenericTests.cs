#if !ROSLYN4_0
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class ExportedConventionsGenericTests(ITestOutputHelper outputHelper) : GeneratorTest(outputHelper)
{
    [Fact]
    public async Task Should_Pull_Through_A_Convention()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention<Contrib>]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_Custom_Namespace()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: ExportConventions(Namespace = ""Source.Space"", ClassName = ""SourceClass"")]
[assembly: Convention<Contrib>]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_No_Namespace()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: ExportConventions(Namespace = null)]
[assembly: Convention<Contrib>]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }


    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_Custom_MethodName()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: ExportConventions(MethodName = ""SourceMethod"")]
[assembly: Convention<Contrib>]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Pull_Through_A_Convention_With_ExportAttribute()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Conventions.Tests
{
    [ExportConventionAttribute]
    internal class Contrib : IConvention { }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Pull_Through_All_Conventions()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;

[assembly: ExportConventions(Namespace = ""Source.Space"")]
[assembly: Convention<Contrib1>]

internal class Contrib1 : IConvention { }
",
                               @"
using Rocket.Surgery.Conventions;

[assembly: Convention<Contrib3>]

[ExportConventionAttribute]
internal class Contrib2 : IConvention { }
internal class Contrib3 : IConvention { }
",
                               @"
using Rocket.Surgery.Conventions;

[assembly: Convention<Contrib4>]

internal class Contrib4 : IConvention { }
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Handle_Duplicate_Conventions()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention<Contrib>]
[assembly: Convention<Contrib>]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Handle_Conventions_With_One_Constructor()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention<ParentContrib.Contrib>]

namespace Rocket.Surgery.Conventions.Tests
{
    interface IService {}
    interface IServiceB {}
    interface IServiceC {}
    internal class ParentContrib {
        internal class Contrib : IConvention { public Contrib(IService service, IServiceB serviceB, IServiceC? serviceC = null) {} }
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Handle_Nested_Conventions()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention<ParentContrib.Contrib>]
[assembly: Convention<ParentContrib.Contrib>]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class ParentContrib {
        internal class Contrib : IConvention { }
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Handle_Nested_Static_Conventions()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention<ParentContrib.Contrib>]
[assembly: Convention<ParentContrib.Contrib>]

namespace Rocket.Surgery.Conventions.Tests
{
    internal static class ParentContrib {
        internal class Contrib : IConvention { }
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Handle_Conventions_With_Nullable_Constructor_Parameters()
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.Mapping;

[assembly: Convention<AutoMapperConvention>]

namespace Rocket.Surgery.LaunchPad.Mapping;

/// <summary>
///     AutoMapperConvention.
///     Implements the <see cref=""IServiceConvention"" />
/// </summary>
/// <seealso cref=""IServiceConvention"" />
public class AutoMapperConvention : IServiceConvention
{
    private readonly AutoMapperOptions _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref=""AutoMapperConvention"" /> class.
    /// </summary>
    /// <param name=""options"">The options.</param>
    public AutoMapperConvention(AutoMapperOptions? options = null)
    {
        _options = options ?? new AutoMapperOptions();
    }

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name=""context"">The context.</param>
    /// <param name=""configuration""></param>
    /// <param name=""services""></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
    }
}

/// <summary>
///     Class AutoMapperOptions.
/// </summary>
public class AutoMapperOptions
{
    /// <summary>
    ///     Gets or sets the service lifetime.
    /// </summary>
    /// <value>The service lifetime.</value>
    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
}
"
                           )
                          .AddReferences(typeof(ServiceLifetime))
                          .Build()
                          .GenerateAsync();
        await Verify(result);
    }

    [Theory]
    [InlineData(HostType.Live)]
    [InlineData(HostType.UnitTest)]
    public async Task Should_Support_HostType_Conventions(HostType hostType)
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention<Contrib>]

namespace Rocket.Surgery.Conventions.Tests
{
    [{HostType}Convention]
    internal class Contrib : IConvention { }
}
".Replace("{HostType}", hostType.ToString(), StringComparison.OrdinalIgnoreCase)
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result).UseTextForParameters(hostType.ToString());
    }

    [Theory]
    [InlineData("AfterConventionAttribute")]
    [InlineData("DependsOnConventionAttribute")]
    [InlineData("BeforeConventionAttribute")]
    [InlineData("DependentOfConventionAttribute")]
    public async Task Should_Support_DependencyDirection_Conventions(string attributeName)
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention<Contrib>]

namespace Rocket.Surgery.Conventions.Tests
{
    [{AttributeName}<D>]
    [LiveConvention, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class Contrib : IConvention { }

    internal class D : IConvention { }
}
".Replace("{AttributeName}", attributeName, StringComparison.OrdinalIgnoreCase)
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result).UseTextForParameters(attributeName);
    }
}
#endif
