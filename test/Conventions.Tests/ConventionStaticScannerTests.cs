using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class ConventionStaticScannerTests : AutoFakeTest
{
    [Fact]
    public void ShouldConstruct()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().WithConventionsFrom(Imports.GetConventions);
        scanner.Should().NotBeNull();
    }

    [Fact]
    public void ShouldBuildAProvider()
    {
        var scanner = ConventionContextHelpers.CreateProvider(
            new ConventionContextBuilder(new Dictionary<object, object?>()).WithConventionsFrom(Imports.GetConventions),
            new TestAssemblyProvider(),
            Logger
        );

        scanner.Get<IServiceConvention, ServiceConvention>()
               .Should()
               .Contain(x => x!.GetType() == typeof(Contrib));
    }

    [Fact]
    public void ShouldScanAddedContributions()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().WithConventionsFrom(Imports.GetConventions);

        var contribution = A.Fake<IServiceConvention>();
        var contribution2 = A.Fake<IServiceConvention>();

        scanner.PrependConvention(contribution);
        scanner.AppendConvention(contribution2);

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<IAssemblyCandidateFinder>(), Logger);

        provider.Get<IServiceConvention, ServiceConvention>()
                .Should()
                .ContainInOrder(contribution, contribution2);
    }

    [Fact]
    public void ShouldIncludeAddedDelegates()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().WithConventionsFrom(Imports.GetConventions);

        var @delegate = new ServiceConvention((_, _, _) => { });
        var delegate2 = new ServiceConvention((_, _, _) => { });

        scanner.PrependDelegate(delegate2);
        scanner.AppendDelegate(@delegate);

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<IAssemblyCandidateFinder>(), Logger);

        provider.Get<IServiceConvention, ServiceConvention>()
                .Should()
                .ContainInOrder(delegate2, @delegate);
    }

    [Fact]
    public void ShouldScanExcludeContributionTypes()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().WithConventionsFrom(Imports.GetConventions);

        var contribution = A.Fake<IServiceConvention>();
        var contribution2 = A.Fake<IServiceConvention>();

        scanner.AppendConvention(contribution);
        scanner.PrependConvention(contribution2);
        scanner.ExceptConvention(typeof(Contrib));

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<IAssemblyCandidateFinder>(), Logger);

        provider.Get<IServiceConvention, ServiceConvention>()
                .Should()
                .NotContain(x => x!.GetType() == typeof(Contrib));
        provider.Get<IServiceConvention, ServiceConvention>()
                .Should()
                .ContainInOrder(contribution2, contribution);
    }

    [Fact]
    public void ShouldScanExcludeContributionAssemblies()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().WithConventionsFrom(Imports.GetConventions);

        var contribution = A.Fake<IServiceConvention>();

        scanner.PrependConvention(contribution);
        scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<IAssemblyCandidateFinder>(), Logger);

        provider.Get<IServiceConvention, ServiceConvention>()
                .Should()
                .NotContain(x => x!.GetType() == typeof(Contrib));
    }


    [Fact]
    public void ShouldScanIncludeContributionTypes()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().WithConventionsFrom(Imports.GetConventions);

        var contribution = A.Fake<IServiceConvention>();
        var contribution2 = A.Fake<IServiceConvention>();

        scanner.AppendConvention(contribution);
        scanner.PrependConvention(contribution2);
        scanner.IncludeConvention(typeof(Contrib));

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<IAssemblyCandidateFinder>(), Logger);

        provider.Get<IServiceConvention, ServiceConvention>()
                .Should()
                .Contain(x => x!.GetType() == typeof(Contrib));
        provider.Get<IServiceConvention, ServiceConvention>()
                .Should()
                .ContainInOrder(contribution2, contribution);
    }

    [Fact]
    public void ShouldScanIncludeContributionAssemblies()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>().WithConventionsFrom(Imports.GetConventions);
        var contribution = A.Fake<IServiceConvention>();

        scanner.PrependConvention(contribution);
        scanner.IncludeConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

        var provider = ConventionContextHelpers.CreateProvider(scanner, A.Fake<IAssemblyCandidateFinder>(), Logger);

        provider.Get<IServiceConvention, ServiceConvention>()
                .Should()
                .Contain(x => x!.GetType() == typeof(Contrib));
    }

    public ConventionStaticScannerTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
}
