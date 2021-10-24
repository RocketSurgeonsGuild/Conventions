using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.WebAssembly.Hosting.Tests;

[assembly: Convention(typeof(AbcConvention))]

namespace Rocket.Surgery.WebAssembly.Hosting.Tests;

public class AbcConvention : IServiceConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }
    }
}
