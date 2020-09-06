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
        public void Should_Have_Imports_Method_Defined_On_Class()
        {
            var method = typeof(StaticConventionTests).GetMethod("GetConventions").Should().NotBeNull().And.Subject;

            var list = method.Invoke(null, new object[] { new ServiceCollection().BuildServiceProvider() })
               .Should().NotBeNull().And.Subject;

            list.As<IEnumerable<IConventionWithDependencies>>().Should().NotBeNull();
        }
    }
}