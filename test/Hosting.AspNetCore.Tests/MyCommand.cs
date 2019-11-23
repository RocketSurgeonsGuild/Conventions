using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Hosting.AspNetCore.Tests
{
    [Command]
    internal class MyCommand
    {
        [UsedImplicitly]
        private readonly IServiceProvider _serviceProvider;

        public MyCommand(IServiceProvider serviceProvider) => _serviceProvider =
            serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        [UsedImplicitly]
        public Task<int> OnExecuteAsync() => Task.FromResult(1234);
    }

    #pragma warning disable CA1801
    internal class MyStartup : IServiceConvention
    {
        public void Configure(IApplicationBuilder builder) { }

        public void Register(IServiceConventionContext context) => context.Services.AddSingleton(new object());
    }
}