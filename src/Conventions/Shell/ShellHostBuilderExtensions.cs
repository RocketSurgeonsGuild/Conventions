using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Rocket.Surgery.Conventions.Shell
{
    /// <summary>
    /// Helper method for working with <see cref="IConventionHostBuilder" />
    /// </summary>
    public static class ShellHostBuilderExtensions
    {
        /// <summary>
        /// Configure the commandline delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder ConfigureShell([NotNull] this IConventionHostBuilder container, ShellConventionDelegate @delegate)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Scanner.AppendDelegate(@delegate);
            return container;
        }
    }


    public class ShellInvocationPipeline
    {
        private InvocationContext _context;

        public ShellInvocationPipeline(InvocationContext context)
        {
            _context = context;
        }

        public async Task<int> InvokeAsync()
        {
            InvocationMiddleware invocationChain = BuildInvocationChain(_context);

            await invocationChain(_context, invocationContext => Task.CompletedTask).ConfigureAwait(false);

            return GetResultCode(_context);
        }

        private static InvocationMiddleware BuildInvocationChain(InvocationContext context)
        {
            var invocations = new List<InvocationMiddleware>((context.Parser.Configuration.GetType().GetProperty("Middleware", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(context.Parser.Configuration) as IReadOnlyCollection<InvocationMiddleware>)!);

            invocations.Add(async (invocationContext, next) =>
            {
                if (invocationContext
                    .ParseResult
                    .CommandResult
                    .Command is Command command)
                {
                    var handler = command.Handler;

                    if (handler != null)
                    {
                        context.ResultCode = await handler.InvokeAsync(invocationContext).ConfigureAwait(false);
                    }
                }
            });

            return invocations.Aggregate(
                (first, second) =>
                    (ctx, next) =>
                        first(ctx,
                            c => second(c, next)));
        }

        private static int GetResultCode(InvocationContext context)
        {
            context.InvocationResult?.Apply(context);

            return context.ResultCode;
        }
    }
}