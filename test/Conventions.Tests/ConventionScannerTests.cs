using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Sample.DependencyOne;
using Sample.DependencyThree;
using Sample.DependencyTwo;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionScannerTests : AutoTestBase
    {
        public ConventionScannerTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace)
        {
        }

        private class Scanner : ConventionScannerBase
        {
            public Scanner(IAssemblyCandidateFinder assemblyCandidateFinder, IServiceProvider serviceProvider, ILogger logger) : base(assemblyCandidateFinder, serviceProvider, logger)
            {
            }
        }

        class C : IServiceConvention
        {
            public void Register(IServiceConventionContext context)
            {
                throw new NotImplementedException();
            }
        }

        class D : IServiceConvention
        {
            public void Register(IServiceConventionContext context)
            {
                throw new NotImplementedException();
            }
        }

        class E : IServiceConvention
        {
            public void Register(IServiceConventionContext context)
            {
                throw new NotImplementedException();
            }
        }

        public interface IService
        {
        }

        class F : IServiceConvention
        {
            public IConventionScanner Scanner { get; }
            public IService Service { get; }

            public F(IConventionScanner scanner, IService service)
            {
                Scanner = scanner;
                Service = service;
            }

            public void Register(IServiceConventionContext context)
            {
            }
        }

        class F_WithDefault : IServiceConvention
        {
            public IConventionScanner Scanner { get; }
            public IService? Service { get; }

            public F_WithDefault(IConventionScanner scanner, IService? service = null)
            {
                Scanner = scanner;
                Service = service;
            }

            public void Register(IServiceConventionContext context)
            {
            }
        }

        [Fact]
        public void ShouldConstruct()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            scanner.Should().NotBeNull();
        }

        [Fact]
        public void ShouldBuildAProvider()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var provider = scanner.BuildProvider();

            var items = provider.Get<IServiceConvention, ServiceConventionDelegate>().ToArray();

            items
                .Select(x => x.Convention)
                .Should()
                .Contain(x => x!.GetType() == typeof(Contrib));
        }

        [Fact]
        public void ShouldCacheTheProvider()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var provider = scanner.BuildProvider();
            var provider2 = scanner.BuildProvider();

            provider.Should().BeSameAs(provider2);
        }

        [Fact]
        public void ShouldScanAddedConventions()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var convention = A.Fake<IServiceConvention>();
            var convention2 = A.Fake<IServiceConvention>();

            scanner.PrependConvention(convention);
            scanner.AppendConvention(convention2);

            var provider = scanner.BuildProvider();

            var items = provider.Get<IServiceConvention, ServiceConventionDelegate>().ToArray();
            foreach (var item in items) Logger.LogInformation("Convention: {@item}", new
            {
                convention = item.Convention?.GetType().FullName,
                @delegate = item.Delegate?.ToString()
            });

            items
                .Select(x => x.Convention)
                .Should()
                .ContainInOrder(convention, convention2);
        }

        [Fact]
        public void ShouldReturnAllConventions()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            IConvention convention = A.Fake<IServiceConvention>();
            IConvention convention2 = A.Fake<ITestConvention>();
            IConvention convention3 = A.Fake<IServiceConvention>();
            IConvention convention4 = A.Fake<ITestConvention>();

            scanner.PrependConvention(convention, convention2);
            scanner.AppendConvention(convention3, convention4);

            var provider = scanner.BuildProvider();

            var result = provider.GetAll()
                .Select(x => x.Convention);

            result.Should().ContainInOrder(convention, convention2, convention3, convention4);
        }

        [Fact]
        public void ShouldReturnAllDelegates()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            Delegate delegate2 = A.Fake<ServiceConventionDelegate>();
            Delegate Delegate = A.Fake<Action>();

            scanner.PrependDelegate(Delegate, delegate2);

            var provider = scanner.BuildProvider();

            var result = provider.GetAll()
                .Select(x => x.Delegate);

            result.Should().Contain(Delegate).And.Contain(delegate2);
        }

        [Fact]
        public void ShouldScanExcludeConventionTypes()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var convention = A.Fake<IServiceConvention>();

            scanner.PrependConvention(convention);
            scanner.ExceptConvention(typeof(ConventionScannerTests));

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(x => x!.GetType() == typeof(Contrib));
        }

        [Fact]
        public void ShouldScanExcludeConventionAssemblies()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            A.CallTo(() => AutoFake.Resolve<IAssemblyCandidateFinder>().GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var convention = A.Fake<IServiceConvention>();

            scanner.PrependConvention(convention);
            scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(x => x!.GetType() == typeof(Contrib));
        }

        [Fact]
        public void ShouldAppendConventions_AsASingle()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            var convention = A.Fake<IServiceConvention>();

            scanner.AppendConvention(convention);

            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();
            result.Count().Should().Be(1);
            result.Select(x => x.Convention).Should().Contain(convention);
        }

        [Fact]
        public void ShouldAppendConventions_AsAnArray()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            scanner.AppendConvention(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldAppendConventions_AsAnEnumerable()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 }.AsEnumerable();

            scanner.AppendConvention(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldAppendConventions_AsAType()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            scanner.AppendConvention<C>();
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(1);
            result.Select(x => x.Convention).Should().AllBeOfType(typeof(C));
        }

        [Fact]
        public void ShouldAppendConventions_AsAnArrayOfTypes()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            scanner.AppendConvention(typeof(C), typeof(E), typeof(D));
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention!.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldAppendConventions_AsAnEnumerableOfTypes()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            scanner.AppendConvention(new[] { typeof(C), typeof(E), typeof(D) }.AsEnumerable());
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention!.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldAppendDelegates_AsAnArray()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            scanner.AppendDelegate(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Delegate).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldAppendDelegates_AsAnEnumerable()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 }.AsEnumerable();

            scanner.AppendDelegate(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Delegate).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldPrependConventions_AsASingle()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            var convention = A.Fake<IServiceConvention>();

            scanner.PrependConvention(convention);

            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();
            result.Count().Should().Be(1);
            result.Select(x => x.Convention).Should().Contain(convention);
        }

        [Fact]
        public void ShouldPrependConventions_AsAnArray()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            scanner.PrependConvention(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldPrependConventions_AsAnEnumerable()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 }.AsEnumerable();

            scanner.PrependConvention(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldPrependConventions_AsAType()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            scanner.PrependConvention<C>();
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(1);
            result.Select(x => x.Convention).Should().AllBeOfType(typeof(C));
        }

        [Fact]
        public void ShouldPrependConventions_AsAnArrayOfTypes()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            scanner.PrependConvention(typeof(C), typeof(E), typeof(D));
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention!.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldPrependConventions_AsAnEnumerableOfTypes()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            scanner.PrependConvention(new[] { typeof(C), typeof(E), typeof(D) }.AsEnumerable());
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Convention!.GetType()).Should().ContainInOrder(typeof(C), typeof(E), typeof(D));
        }

        [Fact]
        public void ShouldPrependDelegates_AsAnArray()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            scanner.PrependDelegate(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Delegate).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldPrependDelegates_AsAnEnumerable()
        {
            var scanner = AutoFake.Resolve<Scanner>();

            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 }.AsEnumerable();

            scanner.PrependDelegate(conventions);
            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            result.Count().Should().Be(3);
            result.Select(x => x.Delegate).Should().ContainInOrder(conventions);
        }

        [Fact]
        public void ShouldResolveConventionsUsingTheServiceProvider_And_Fail_IfTypeIsNotProvided()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProvider>(properties);
            var scanner = AutoFake.Resolve<Scanner>();
            properties.Add(typeof(IConventionScanner), scanner);

            scanner.AppendConvention<F>();

            Action a = () => scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();
            a.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ShouldResolveConventionsUsingTheServiceProvider()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProvider>(properties);
            var scanner = AutoFake.Resolve<Scanner>();
            properties.Add(typeof(IConventionScanner), scanner);
            var fakeService = A.Fake<IService>();
            properties.Add(typeof(IService), fakeService);

            scanner.AppendConvention<F>();

            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            var item = result.First().Convention;

            item.Should().BeOfType<F>();
            (item as F)!.Scanner.Should().NotBeNull();
            (item as F)!.Scanner.Should().BeSameAs(scanner);
            (item as F)!.Service.Should().BeSameAs(fakeService);
        }

        [Fact]
        public void ShouldResolveConventionsUsingTheServiceProvider_IgnoringDefaultValues()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProvider>(properties);
            var scanner = AutoFake.Resolve<Scanner>();
            properties.Add(typeof(IConventionScanner), scanner);

            scanner.AppendConvention<F_WithDefault>();

            var result = scanner.BuildProvider().Get<IServiceConvention, ServiceConventionDelegate>();

            var item = result.First().Convention;

            item.Should().BeOfType<F_WithDefault>();
            (item as F_WithDefault)!.Scanner.Should().NotBeNull();
            (item as F_WithDefault)!.Scanner.Should().BeSameAs(scanner);
            (item as F_WithDefault)!.Service.Should().BeNull();
        }

        [Fact]
        public void ShouldExcludeScannedItemsIfAddedManually()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProvider>(properties);
            var scanner = AutoFake.Resolve<Scanner>();
            var finder = AutoFake.Resolve<IAssemblyCandidateFinder>();

            var myConvention1 = new Class1();
            var myConvention2 = new Class2();
            var myConvention3 = new Class3();

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly, typeof(Class1).GetTypeInfo().Assembly, typeof(Class2).GetTypeInfo().Assembly, typeof(Class3).GetTypeInfo().Assembly });

            scanner.AppendConvention(myConvention1, myConvention3, myConvention2);
            scanner.ExceptConvention(typeof(ConventionScannerTests).Assembly);

            var provider = scanner.BuildProvider();

            var result = provider.GetAll()
                .Select(x => x.Convention)
                .ToArray();

            result.Should().ContainInOrder(myConvention1, myConvention3, myConvention2);
        }
    }
}
