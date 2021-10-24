using McMaster.Extensions.CommandLineUtils;

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     Adds a help option of --help if no other help option is specified.
///     Implements the <see cref="McMaster.Extensions.CommandLineUtils.Conventions.IConvention" />
/// </summary>
/// <seealso cref="McMaster.Extensions.CommandLineUtils.Conventions.IConvention" />
internal class DefaultHelpOptionConvention : McMaster.Extensions.CommandLineUtils.Conventions.IConvention
{
    /// <summary>
    ///     The default help template.
    /// </summary>
    public const string DefaultHelpTemplate = "-?|-h|--help";

    /// <summary>
    ///     Applies the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <inheritdoc />
    public void Apply(McMaster.Extensions.CommandLineUtils.Conventions.ConventionContext context)
    {
        if (context.Application.OptionHelp != null)
        {
            return;
        }

        var help = new CommandOption(DefaultHelpTemplate, CommandOptionType.NoValue)
        {
            Description = "Show help information",

            // the convention will run on each subcommand automatically.
            // it is better to run the command on each to check for overlap
            // or already set options to avoid conflict
            Inherited = false
        };

        foreach (var opt in context.Application.GetOptions())
        {
            if (string.Equals(help.LongName, opt.LongName, StringComparison.OrdinalIgnoreCase))
            {
                help.LongName = null;
            }

            if (string.Equals(help.ShortName, opt.ShortName, StringComparison.OrdinalIgnoreCase))
            {
                help.ShortName = null;
            }

            if (string.Equals(help.SymbolName, opt.SymbolName, StringComparison.OrdinalIgnoreCase))
            {
                help.SymbolName = null;
            }
        }

        if (help.LongName != null || help.ShortName != null || help.SymbolName != null)
        {
            context.Application.HelpOption();
        }
    }
}
