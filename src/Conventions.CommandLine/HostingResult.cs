using Spectre.Console.Cli;

namespace Rocket.Surgery.Conventions.CommandLine;

class HostingResult
{
    public IDictionary<string, string>? Configuration { get; set; }
    public IRemainingArguments? Arguments { get; set; }
}