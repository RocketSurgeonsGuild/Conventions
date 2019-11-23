using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable CoVariantArrayConversion

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionHostBuilderTests : AutoFakeTest
    {
        [Fact]
        public void ShouldConstruct()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();

            builder.Should().NotBeNull();
            builder.Properties.Should().NotBeNull();
            builder.Should().NotBeNull();
            builder.AssemblyCandidateFinder.Should().NotBeNull();
            builder.AssemblyProvider.Should().NotBeNull();

            builder["a"] = "b";

            builder["a"].Should().Be("b");
        }

        [Fact]
        public void ShouldAppendConventions_AsASingle()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();
            var convention = A.Fake<IServiceConvention>();

            builder.AppendConvention(convention);

            A.CallTo(() => builder.Scanner.AppendConvention(A<IConvention[]>.Ignored))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldAppendConventions_AsAnArray()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();
            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            builder.AppendConvention(conventions);

            A.CallTo(() => builder.Scanner.AppendConvention(A<IConvention[]>.Ignored))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldAppendConventions_AsAnEnumerable()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();
            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 }.AsEnumerable();

            builder.AppendConvention(conventions);

            A.CallTo(() => builder.Scanner.AppendConvention(A<IEnumerable<IConvention>>.Ignored))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldAppendConventions_AsAType()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();

            builder.AppendConvention<C>();

            A.CallTo(() => builder.Scanner.AppendConvention<C>()).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldAppendConventions_AsAnArrayOfTypes()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();

            builder.AppendConvention(typeof(C), typeof(E), typeof(D));

            A.CallTo(() => builder.Scanner.AppendConvention(A<Type[]>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldAppendConventions_AsAnEnumerableOfTypes()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();

            builder.AppendConvention(new[] { typeof(C), typeof(E), typeof(D) }.AsEnumerable());

            A.CallTo(() => builder.Scanner.AppendConvention(A<IEnumerable<Type>>.Ignored))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldAppendDelegates_AsAnArray()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();

            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            builder.AppendDelegate(conventions);

            A.CallTo(() => builder.Scanner.AppendDelegate(A<Delegate[]>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldAppendDelegates_AsAnEnumerable()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();

            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new Delegate[] { convention3, convention, convention2 }.AsEnumerable();

            builder.AppendDelegate(conventions);

            A.CallTo(() => builder.Scanner.AppendDelegate(A<IEnumerable<Delegate>>.Ignored))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldPrependConventions_AsASingle()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();
            var convention = A.Fake<IServiceConvention>();

            builder.PrependConvention(convention);

            A.CallTo(() => builder.Scanner.PrependConvention(A<IConvention[]>.Ignored))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldPrependConventions_AsAnArray()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();

            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            builder.PrependConvention(conventions);

            A.CallTo(() => builder.Scanner.PrependConvention(A<IConvention[]>.Ignored))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldPrependConventions_AsAnEnumerable()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();

            var convention = A.Fake<IServiceConvention>(x => x.Named("convention"));
            var convention2 = A.Fake<IServiceConvention>(x => x.Named("convention2"));
            var convention3 = A.Fake<IServiceConvention>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 }.AsEnumerable();

            builder.PrependConvention(conventions);

            A.CallTo(() => builder.Scanner.PrependConvention(A<IEnumerable<IConvention>>.Ignored))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldPrependConventions_AsAType()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();


            builder.PrependConvention<C>();

            A.CallTo(() => builder.Scanner.PrependConvention<C>()).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldPrependConventions_AsAnArrayOfTypes()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();


            builder.PrependConvention(typeof(C), typeof(E), typeof(D));

            A.CallTo(() => builder.Scanner.PrependConvention(A<Type[]>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldPrependConventions_AsAnEnumerableOfTypes()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();


            builder.PrependConvention(new[] { typeof(C), typeof(E), typeof(D) }.AsEnumerable());

            A.CallTo(() => builder.Scanner.PrependConvention(A<IEnumerable<Type>>.Ignored))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldPrependDelegates_AsAnArray()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();

            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 };

            builder.PrependDelegate(conventions);

            A.CallTo(() => builder.Scanner.PrependDelegate(A<Delegate[]>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldPrependDelegates_AsAnEnumerable()
        {
            var properties = new ServiceProviderDictionary();
            AutoFake.Provide<IServiceProviderDictionary>(properties);
            using var diaglosticSource = new DiagnosticListener("DiagnosticSource");
            AutoFake.Provide<DiagnosticSource>(diaglosticSource);
            var builder = AutoFake.Resolve<CCBuilder>();

            var convention = A.Fake<ServiceConventionDelegate>(x => x.Named("convention"));
            var convention2 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention2"));
            var convention3 = A.Fake<ServiceConventionDelegate>(x => x.Named("convention3"));

            var conventions = new[] { convention3, convention, convention2 }.AsEnumerable();

            builder.PrependDelegate(conventions);

            A.CallTo(() => builder.Scanner.PrependDelegate(A<IEnumerable<Delegate>>.Ignored))
               .MustHaveHappened(1, Times.Exactly);
        }

        public ConventionHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        private class CCBuilder : ConventionHostBuilder<CCBuilder>
        {
            public CCBuilder(
                IConventionScanner scanner,
                IAssemblyCandidateFinder assemblyCandidateFinder,
                IAssemblyProvider assemblyProvider,
                DiagnosticSource diagnosticSource,
                IServiceProviderDictionary properties
            ) : base(scanner, assemblyCandidateFinder, assemblyProvider, diagnosticSource, properties) { }
        }

        private class C : IServiceConvention
        {
            public void Register(IServiceConventionContext context) => throw new NotImplementedException();
        }

        private class D : IServiceConvention
        {
            public void Register(IServiceConventionContext context) => throw new NotImplementedException();
        }

        private class E : IServiceConvention
        {
            public void Register(IServiceConventionContext context) => throw new NotImplementedException();
        }
    }
}