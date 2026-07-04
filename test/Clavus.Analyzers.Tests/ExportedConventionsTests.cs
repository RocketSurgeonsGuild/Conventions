using Microsoft.Extensions.DependencyInjection;

namespace Clavus.Analyzers.Tests;

public class ExportedConventionsTests() : GeneratorTest()
{
    [Test]
    public async Task Should_Pull_Through_A_Convention()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;

namespace Clavus.Tests
{
    [ClavusExport]
    internal class Contrib : IClavusPart { }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Pull_Through_A_Convention_With_Custom_Namespace()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;


namespace Clavus.Tests
{
    [ClavusExport]
    internal class Contrib : IClavusPart { }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Pull_Through_A_Convention_With_No_Namespace()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;


namespace Clavus.Tests
{
    [ClavusExport]
    internal class Contrib : IClavusPart { }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }


    [Test]
    public async Task Should_Pull_Through_A_Convention_With_Custom_MethodName()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;


namespace Clavus.Tests
{
    [ClavusExport]
    internal class Contrib : IClavusPart { }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Pull_Through_A_Convention_With_ExportAttribute()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;

namespace Clavus.Tests
{
    [ClavusExportAttribute]
    internal class Contrib : IClavusPart { }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Pull_Through_All_Conventions()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;


[ClavusExport]
internal class Contrib1 : IClavusPart { }
",
                               @"
using Clavus;

[ClavusExportAttribute]
internal class Contrib2 : IClavusPart { }
[ClavusExport]
internal class Contrib3 : IClavusPart { }
",
                               @"
using Clavus;

[ClavusExport]
internal class Contrib4 : IClavusPart { }
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Handle_Conventions_With_One_Constructor()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;

namespace Clavus.Tests
{
    interface IService {}
    interface IServiceB {}
    interface IServiceC {}
    internal class ParentContrib {
        [ClavusExport]
        internal class Contrib : IClavusPart { public Contrib(IService service, IServiceB serviceB, IServiceC? serviceC = null) {} }
    }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Handle_Nested_Conventions()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;

namespace Clavus.Tests
{
    internal class ParentContrib {
        [ClavusExport]
        internal class Contrib : IClavusPart { }
    }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Handle_Nested_Static_Conventions()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;

namespace Clavus.Tests
{
    internal static class ParentContrib {
        [ClavusExport]
        internal class Contrib : IClavusPart { }
    }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    public async Task Should_Handle_Conventions_With_Nullable_Constructor_Parameters()
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Clavus;
using Rocket.Surgery.LaunchPad.Mapping;

namespace Rocket.Surgery.LaunchPad.Mapping;

/// <summary>
///     AutoMapperConvention.
///     Implements the <see cref=""IServicePart"" />
/// </summary>
/// <seealso cref=""IServicePart"" />
[ClavusExport]
public class AutoMapperConvention : IServicePart
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
    public void Register(IClavusContext context, IConfiguration configuration, IServiceCollection services)
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
                          .GenerateAsync(TestContext.CancellationToken);
        await Verify(result);
    }

    [Test]
    [Arguments(HostType.Live)]
    [Arguments(HostType.UnitTest)]
    public async Task Should_Support_HostType_Conventions(HostType hostType)
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;

namespace Clavus.Tests
{
    [ClavusExport]
    [{HostType}Convention]
    internal class Contrib : IClavusPart { }
}
".Replace("{HostType}", hostType.ToString(), StringComparison.OrdinalIgnoreCase)
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseTextForParameters(hostType.ToString());
    }

    [Test]
    [Arguments("Custom")]
    [Arguments("Infrastructure")]
    [Arguments("Application")]
    public async Task Should_Support_Category_Conventions(string category)
    {
        var result = await WithGenericSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;

namespace Clavus.Tests
{
    [ClavusExport]
    [ClavusCategory(""{Category}"")]
    internal class Contrib : IClavusPart { }
}
".Replace("{Category}", category, StringComparison.OrdinalIgnoreCase)
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseTextForParameters(category);
    }

    [Test]
    [Arguments("AfterPartAttribute")]
    [Arguments("DependsOnPartAttribute")]
    [Arguments("BeforePartAttribute")]
    [Arguments("DependentOfPartAttribute")]
    public async Task Should_Support_DependencyDirection_Conventions(string attributeName)
    {
        var result = await WithSharedDeps()
                          .AddSources(
                               @"
using Clavus;
using Clavus.Tests;

namespace Clavus.Tests
{
    [ClavusExport]
    [{AttributeName}(typeof(D))]
    [LivePart, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class Contrib : IClavusPart { }

    internal class D : IClavusPart { }
}
".Replace("{AttributeName}", attributeName, StringComparison.OrdinalIgnoreCase)
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseTextForParameters(attributeName);
    }

    [Before(Test)]
    public Task InitializeAsync()
    {
        Configure(b => b.IgnoreOutputFile("Imported_Assembly_Conventions.cs"));
        return Task.CompletedTask;
    }
}
