using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Extensions;

internal class LoggingBuilder(IServiceCollection services) : ILoggingBuilder
{
    public IServiceCollection Services { get; } = services;
}
