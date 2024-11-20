using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.DependencyInjection.Compiled;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class ConventionStaticScannerTests(ITestOutputHelper outputHelper) : AutoFakeTest<LocalTestContext>(LocalTestContext.Create(outputHelper))
{
    [field: AllowNull, MaybeNull]
    private ILoggerFactory LoggerFactory => field ??= CreateLoggerFactory();
    private ILogger Logger => LoggerFactory.CreateLogger(GetType());

    [Fact]
    public void ShouldConstruct()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().UseConventionFactory(Imports.Instance);
        scanner.Should().NotBeNull();
    }

    [Fact]
    public void ShouldBuildAProvider()
    {
        var scanner = ConventionContextHelpers.CreateProvider(
            new ConventionContextBuilder(new Dictionary<object, object?>(), []).UseConventionFactory(Imports.Instance),
            new TestAssemblyProvider(),
            Logger
        );

        scanner
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .Contain(x => x is Contrib);
    }

    [Fact]
    public void ShouldScanAddedContributions()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().UseConventionFactory(Imports.Instance);

        var contribution = A.Fake<IServiceConvention>();
        var contribution2 = A.Fake<IServiceConvention>();

        scanner.PrependConvention(contribution);
        scanner.AppendConvention(contribution2);

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<ICompiledTypeProvider>(), Logger);

        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .ContainInOrder(contribution, contribution2);
    }

    [Fact]
    public void ShouldIncludeAddedDelegates()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().UseConventionFactory(Imports.Instance);

        var @delegate = new ServiceConvention((_, _, _) => { });
        var delegate2 = new ServiceConvention((_, _, _) => { });

        scanner.PrependDelegate(delegate2, null, null);
        scanner.AppendDelegate(@delegate, null, null);

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<ICompiledTypeProvider>(), Logger);

        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .ContainInOrder(delegate2, @delegate);
    }

    [Fact]
    public void ShouldScanExcludeContributionTypes()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().UseConventionFactory(Imports.Instance);

        var contribution = A.Fake<IServiceConvention>();
        var contribution2 = A.Fake<IServiceConvention>();

        scanner.AppendConvention(contribution);
        scanner.PrependConvention(contribution2);
        scanner.ExceptConvention(typeof(Contrib));

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<ICompiledTypeProvider>(), Logger);

        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .NotContain(x => x is Contrib);
        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .ContainInOrder(contribution2, contribution);
    }

    [Fact]
    public void ShouldScanExcludeContributionAssemblies()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().UseConventionFactory(Imports.Instance);

        var contribution = A.Fake<IServiceConvention>();

        scanner.PrependConvention(contribution);
        scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<ICompiledTypeProvider>(), Logger);

        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .NotContain(x => x is Contrib);
    }


    [Fact]
    public void ShouldScanIncludeContributionTypes()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().UseConventionFactory(Imports.Instance);

        var contribution = A.Fake<IServiceConvention>();
        var contribution2 = A.Fake<IServiceConvention>();

        scanner.AppendConvention(contribution);
        scanner.PrependConvention(contribution2);
        scanner.IncludeConvention(typeof(Contrib).Assembly);

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<ICompiledTypeProvider>(), Logger);

        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .Contain(x => x is Contrib);
        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .ContainInOrder(contribution2, contribution);
    }

    [Fact]
    public void ShouldScanIncludeContributionAssemblies()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().UseConventionFactory(Imports.Instance);
        var contribution = A.Fake<IServiceConvention>();

        scanner.PrependConvention(contribution);
        scanner.IncludeConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<ICompiledTypeProvider>(), Logger);

        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .Contain(x => x is Contrib);
    }
}
