using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Sample;

public class Dump : AsyncCommand<AppSettings>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Dump> _logger;

    public Dump(IConfiguration configuration, ILogger<Dump> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public override Task<int> ExecuteAsync(CommandContext context, AppSettings settings)
    {
        foreach (var item in _configuration.AsEnumerable().Reverse())
        {
            _logger.LogInformation("{Key}: {Value}", item.Key, item.Value ?? string.Empty);
        }

        return Task.FromResult(1);
    }

    [ExportConvention]
    internal class DumpConvention : ICommandLineConvention
    {
        public void Register(IConventionContext context, IConfigurator app)
        {
            app.AddCommand<Dump>("dump");
        }
    }
}
