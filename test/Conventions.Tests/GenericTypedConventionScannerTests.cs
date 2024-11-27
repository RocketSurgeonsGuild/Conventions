using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.DependencyInjection.Compiled;
using Rocket.Surgery.Extensions.Testing;
using Sample.DependencyOne;
using Sample.DependencyThree;
using Sample.DependencyTwo;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class GenericTypedConventionScannerTests(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper))
{
    [Fact]
    public void ShouldConstruct()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>();
        scanner.Should().NotBeNull();
    }

    [Fact]
    public void ShouldBuildAProvider()
    {
        var scanner = ConventionContextHelpers.CreateProvider(
            new ConventionContextBuilder(new Dictionary<object, object?>(), []).AppendConvention(new Contrib()),
            GetType().Assembly.GetCompiledTypeProvider(),
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
        var scanner = AutoFake.Resolve<ConventionContextBuilder>();
        var finder = AutoFake.Resolve<ICompiledTypeProvider>();

        A
            // ReSharper disable ExplicitCallerInfoArgument
           .CallTo(() => finder.GetAssemblies(A<Action<IReflectionAssemblySelector>>._, A<int>._, A<string>._, A<string>._))
            // ReSharper restore ExplicitCallerInfoArgument
           .Returns(Array.Empty<Assembly>());

        var contribution = A.Fake<IServiceConvention>();
        var contribution2 = A.Fake<IServiceConvention>();

        scanner.PrependConvention(contribution);
        scanner.AppendConvention(contribution2);

        var provider = ConventionContextHelpers.CreateProvider(scanner, finder, Logger);

        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .ContainInOrder(contribution, contribution2);
    }

    [Fact]
    public void ShouldIncludeAddedDelegates()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>();
        var finder = AutoFake.Resolve<ICompiledTypeProvider>();

        // ReSharper disable ExplicitCallerInfoArgument
        A
           .CallTo(() => finder.GetAssemblies(A<Action<IReflectionAssemblySelector>>._, A<int>._, A<string>._, A<string>._))
            // ReSharper restore ExplicitCallerInfoArgument
           .Returns(Array.Empty<Assembly>());

        var @delegate = new ServiceConvention((_, _, _) => { });
        var delegate2 = new ServiceConvention((_, _, _) => { });

        scanner.PrependDelegate(delegate2, null, null);
        scanner.AppendDelegate(@delegate, null, null);

        var provider = ConventionContextHelpers.CreateProvider(scanner, finder, Logger);

        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .ContainInOrder(delegate2, @delegate);
    }

    [Fact]
    public void ShouldScanExcludeContributionTypes()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>();
        var finder = AutoFake.Resolve<ICompiledTypeProvider>();

        A
            // ReSharper disable ExplicitCallerInfoArgument
           .CallTo(() => finder.GetAssemblies(A<Action<IReflectionAssemblySelector>>._, A<int>._, A<string>._, A<string>._))
            // ReSharper restore ExplicitCallerInfoArgument
           .Returns(
                new[]
                {
                    typeof(ConventionScannerTests).GetTypeInfo().Assembly, typeof(Class1).GetTypeInfo().Assembly,
                    typeof(Nested.Class2).GetTypeInfo().Assembly, typeof(Class3).GetTypeInfo().Assembly,
                }
            );

        var contribution = A.Fake<IServiceConvention>();
        var contribution2 = A.Fake<IServiceConvention>();

        scanner.AppendConvention(contribution);
        scanner.PrependConvention(contribution2);
        scanner.ExceptConvention(typeof(Contrib));

        var provider = ConventionContextHelpers.CreateProvider(scanner, finder, Logger);

        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .NotContain(x => x! is Contrib);
        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .ContainInOrder(contribution2, contribution);
    }

    [Fact]
    public void ShouldScanExcludeContributionAssemblies()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>();
        var finder = AutoFake.Resolve<ICompiledTypeProvider>();

        A
            // ReSharper disable ExplicitCallerInfoArgument
           .CallTo(() => finder.GetAssemblies(A<Action<IReflectionAssemblySelector>>._, A<int>._, A<string>._, A<string>._))
            // ReSharper restore ExplicitCallerInfoArgument
           .Returns(
                new[]
                {
                    typeof(ConventionScannerTests).GetTypeInfo().Assembly, typeof(Class1).GetTypeInfo().Assembly,
                    typeof(Nested.Class2).GetTypeInfo().Assembly, typeof(Class3).GetTypeInfo().Assembly,
                }
            );

        var contribution = A.Fake<IServiceConvention>();

        scanner.PrependConvention(contribution);
        scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

        var provider = ConventionContextHelpers.CreateProvider(scanner, finder, Logger);

        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .NotContain(x => x! is Contrib);
    }


    [Fact]
    public void ShouldScanIncludeContributionTypes()
    {
        var scanner = AutoFake.Resolve<ConventionContextBuilder>();
        var finder = AutoFake.Resolve<ICompiledTypeProvider>();

        A
            // ReSharper disable ExplicitCallerInfoArgument
           .CallTo(() => finder.GetAssemblies(A<Action<IReflectionAssemblySelector>>._, A<int>._, A<string>._, A<string>._))
            // ReSharper restore ExplicitCallerInfoArgument
           .Returns(
                new[]
                {
                    typeof(Class1).GetTypeInfo().Assembly,
                    typeof(Nested.Class2).GetTypeInfo().Assembly, typeof(Class3).GetTypeInfo().Assembly,
                }
            );

        var contribution = A.Fake<IServiceConvention>();
        var contribution2 = A.Fake<IServiceConvention>();

        scanner.AppendConvention(contribution);
        scanner.PrependConvention(contribution2);
        scanner.IncludeConvention(typeof(Contrib).Assembly);

        var provider = ConventionContextHelpers.CreateProvider(scanner, finder, Logger);

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
        var scanner = AutoFake.Resolve<ConventionContextBuilder>();
        var finder = AutoFake.Resolve<ICompiledTypeProvider>();

        A
            // ReSharper disable ExplicitCallerInfoArgument
           .CallTo(() => finder.GetAssemblies(A<Action<IReflectionAssemblySelector>>._, A<int>._, A<string>._, A<string>._))
            // ReSharper restore ExplicitCallerInfoArgument
           .Returns(
                new[]
                {
                    typeof(Class1).GetTypeInfo().Assembly,
                    typeof(Nested.Class2).GetTypeInfo().Assembly, typeof(Class3).GetTypeInfo().Assembly,
                }
            );

        var contribution = A.Fake<IServiceConvention>();

        scanner.PrependConvention(contribution);
        scanner.IncludeConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

        var provider = ConventionContextHelpers.CreateProvider(scanner, finder, Logger);

        provider
           .Get<IServiceConvention, ServiceConvention>()
           .Should()
           .Contain(x => x is Contrib);
    }

    [field: AllowNull]
    [field: MaybeNull]
    private ILoggerFactory LoggerFactory => field ??= CreateLoggerFactory();

    private ILogger Logger => LoggerFactory.CreateLogger(GetType());
}
