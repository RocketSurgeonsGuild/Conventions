using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Sample
{
    [Command("dump")]
    public class Dump
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Dump> _logger;

        public Dump(IConfiguration configuration, ILogger<Dump> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<int> OnExecuteAsync()
        {
            foreach (var item in _configuration.AsEnumerable().Reverse())
            {
                _logger.LogInformation("{Key}: {Value}", item.Key, item.Value ?? string.Empty);
            }

            return Task.FromResult(1);
        }
    }
}