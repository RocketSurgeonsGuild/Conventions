using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Spectre.Console.Cli;

namespace Sample;

public class Dump(IConfiguration configuration, ILogger<Dump> logger) : AsyncCommand<AppSettings>
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<Dump> _logger = logger;

    public override Task<int> ExecuteAsync(CommandContext context, AppSettings settings, CancellationToken token)
    {
        foreach (var item in _configuration.AsEnumerable().Reverse())
        {
#pragma warning disable CA1848
            _logger.LogInformation("{Key}: {Value}", item.Key, item.Value ?? "");
#pragma warning restore CA1848
        }

        return Task.FromResult(1);
    }

    [ExportConvention]
    internal class DumpConvention : ICommandLineConvention
    {
        public void Register(IConventionContext context, IConfigurator app) => app.AddCommand<Dump>("dump");
    }
}
