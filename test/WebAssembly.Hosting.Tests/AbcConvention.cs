using System;
using FakeItEasy;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.WebAssembly.Hosting.Tests;

[assembly: Convention(typeof(AbcConvention))]

namespace Rocket.Surgery.WebAssembly.Hosting.Tests
{
    public class AbcConvention : IServiceConvention
    {
        public void Register(IServiceConventionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
    }
}