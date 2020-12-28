using System;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Sample.DependencyOne;
using Sample.DependencyThree;
using Sample.DependencyTwo;
using Xunit;
using static Rocket.Surgery.Conventions.Analyzers.Tests.GenerationHelpers;

namespace Rocket.Surgery.Conventions.Analyzers.Tests
{
    public class ImportConventionsTests
    {
        [Fact]
        public async Task Should_Generate_Static_Assembly_Level_Method()
        {
            var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
";
            var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

namespace TestProject.Conventions
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal static partial class Imports
    {
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            foreach (var convention in Sample.DependencyOne.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in Sample.DependencyTwo.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in Sample.DependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";

            await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
                new[] { typeof(Class1).Assembly, typeof(Class2).Assembly, typeof(Class3).Assembly },
                source,
                expected
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_Generate_Static_Class_Member_Level_Method()
        {
            var source = @"
using Rocket.Surgery.Conventions;

namespace TestProject
{
    [ImportConventions]
    public partial class Program
    {
    }
}
";
            var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

namespace TestProject
{
    public partial class Program
    {
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            foreach (var convention in Sample.DependencyOne.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in Sample.DependencyTwo.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in Sample.DependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";

            await AssertGeneratedAsExpected<ConventionAttributesGenerator>(
                new[] { typeof(Class1).Assembly, typeof(Class2).Assembly, typeof(Class3).Assembly },
                source,
                expected
            ).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_Support_No_Exported_Convention_Assemblies()
        {
            var source = @"
using Rocket.Surgery.Conventions;

[assembly: ImportConventions]
";
            var expected = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

namespace TestProject.Conventions
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static class __imports__
    {
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield break;
        }
    }
}
";

            await AssertGeneratedAsExpected<ConventionAttributesGenerator>(source, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task Should_Support_Imports_And_Exports_In_The_Same_Assembly()
        {
            var source1 = @"
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Tests;

[assembly: Convention(typeof(Contrib))]

namespace Rocket.Surgery.Conventions.Tests
{
    internal class Contrib : IConvention { }
}
";
            var source2 = @"
using Rocket.Surgery.Conventions;

namespace TestProject
{
    [ImportConventions]
    public partial class Program
    {
    }
}
";
            var expected1 = @"
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

namespace TestProject
{
    public partial class Program
    {
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            foreach (var convention in TestProject.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}
";
            var expected2 = @"
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;

[assembly: ExportedConventions(typeof(Rocket.Surgery.Conventions.Tests.Contrib))]
namespace TestProject.Conventions
{
    [System.Runtime.CompilerServices.CompilerGenerated]
    public static class Exports
    {
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            yield return new ConventionWithDependencies(ActivatorUtilities.CreateInstance<Rocket.Surgery.Conventions.Tests.Contrib>(serviceProvider), HostType.Undefined);
        }
    }
}
";

            var responses = await Generate<ConventionAttributesGenerator>(
                System.Array.Empty<Assembly>(),
                new[]
                {
                    source1, source2
                }
            ).ConfigureAwait(false);

            foreach (var response in responses)
            {
                if (response.Contains("ExportedConventions", StringComparison.OrdinalIgnoreCase))
                {
                    response.Should().Be(NormalizeToLf(expected2).Trim());
                }
                else if (response.Contains("class Program", StringComparison.OrdinalIgnoreCase))
                {
                    response.Should().Be(NormalizeToLf(expected1).Trim());
                }
                else
                {
                    throw new Exception("Could not find item to compare");
                }
            }
        }
    }
}