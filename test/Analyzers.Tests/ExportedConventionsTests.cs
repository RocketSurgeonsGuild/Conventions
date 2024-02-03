using Microsoft.Extensions.DependencyInjection;
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

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
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

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
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

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
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

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
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

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
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

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps())).UseTextForParameters(hostType.ToString());
    }

    [Theory]
    [InlineData("AfterConventionAttribute")]
    [InlineData("DependsOnConventionAttribute")]
    [InlineData("BeforeConventionAttribute")]
    [InlineData("DependentOfConventionAttribute")]
    public async Task Should_Support_DependencyDirection_Conventions(string attributeName)
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

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps())).UseTextForParameters(attributeName);
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

        await Verify(GenerateAll(new[] { source1, source2, source3 }, compilationReferences: await CreateDeps()));
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

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
    }

    [Fact]
    public async Task Should_Handle_Conventions_With_One_Constructor()
    {
        var source = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(ParentContrib.Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    interface IService {}
    interface IServiceB {}
    interface IServiceC {}
    internal class ParentContrib {
        internal class Contrib : IConvention { public Contrib(IService service, IServiceB serviceB, IServiceC? serviceC = null) {} }
    }
}
";

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
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

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
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

        await Verify(GenerateAll(source, compilationReferences: await CreateDeps()));
    }

    [Fact]
    public async Task Should_Handle_Conventions_With_Nullable_Constructor_Parameters()
    {
        var source = @"
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.Mapping;

[assembly: Convention(typeof(AutoMapperConvention))]

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
        var assemblies = context.AssemblyCandidateFinder.GetCandidateAssemblies(nameof(AutoMapper)).ToArray();
        services.AddAutoMapper(assemblies, _options.ServiceLifetime);
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
";
        await Verify(GenerateAll(source, new[] { typeof(ServiceLifetime).Assembly }));
    }
}
