using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Xunit;

[assembly: ImportConventions]

namespace Rocket.Surgery.Conventions.Tests
{
    [ImportConventions]
    public partial class StaticConventionTests
    {
        [Fact]
        public void Should_Have_Exports_Method_Defined()
        {
            // one day...
            // var a = typeof(Rocket.Surgery.Conventions.Tests.__conventions__.__exports__);
            var exports = typeof(StaticConventionTests).Assembly.GetExportedTypes()
               .FirstOrDefault(x => x.Name == "__exports__").Should().NotBeNull().And.Subject;
            var method = exports.GetMethod("GetConventions").Should().NotBeNull().And.Subject;

            var list = method.Invoke(null, new object[] { new ServiceCollection().BuildServiceProvider() })
               .Should().NotBeNull().And.Subject;

            list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull();
        }

        [Fact]
        public void Should_Have_Imports_Method_Defined_On_Assembly()
        {
            // one day...
            // var a = typeof(Rocket.Surgery.Conventions.Tests.__conventions__.__imports__);
            var exports = typeof(StaticConventionTests).Assembly.GetExportedTypes()
               .FirstOrDefault(x => x.Name == "__imports__").Should().NotBeNull().And.Subject;
            var method = exports.GetMethod("GetConventions").Should().NotBeNull().And.Subject;

            var list = method.Invoke(null, new object[] { new ServiceCollection().BuildServiceProvider() })
               .Should().NotBeNull().And.Subject;

            list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull();
        }

        [Fact]
        public void Should_Have_Imports_Method_Defined_On_Assembly_Into_Provider()
        {
            // one day...
            // var a = typeof(Rocket.Surgery.Conventions.Tests.__conventions__.__imports__);
            var exports = typeof(StaticConventionTests).Assembly.GetExportedTypes()
               .FirstOrDefault(x => x.Name == "__imports__").Should().NotBeNull().And.Subject;
            var method = exports.GetMethod("GetConventions").Should().NotBeNull().And.Subject;

            var list = method.Invoke(null, new object[] { new ServiceCollection().BuildServiceProvider() })
               .Should().NotBeNull().And.Subject;

            var items = list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull().And.Subject;

            var provider = new ConventionProvider(HostType.Undefined, items, Array.Empty<object>(), Array.Empty<object>());
            Func<IEnumerable<object>> a = () => provider.GetAll();

            a.Should().NotThrow();

            var values = a().ToArray();
            values.OfType<Contrib>().Should().HaveCount(1);
        }

        [Fact]
        public void Should_Have_Imports_Method_Defined_On_Class()
        {
            var method = typeof(StaticConventionTests).GetMethod("GetConventions").Should().NotBeNull().And.Subject;

            var list = method.Invoke(null, new object[] { new ServiceCollection().BuildServiceProvider() })
               .Should().NotBeNull().And.Subject;

            list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull();
        }
    }
}