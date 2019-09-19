using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    ///  IHostingConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    public interface IHostingConventionContext : IConventionContext, IRocketHostBuilder
    {
    }
}
