using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Clavus.DryIoc.Tests;

internal sealed class LoggingBuilder(IServiceCollection services) : ILoggingBuilder
{
    public IServiceCollection Services { get; } = services;
}
