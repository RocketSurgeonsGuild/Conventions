using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.WebAssembly.Hosting
{
    internal class WebAssemblyHostingConventionContext : ConventionContext
    {
        private readonly IConventionHostBuilder _builder;
        private readonly IWebAssemblyHostBuilder _hostBuilder;

        public WebAssemblyHostingConventionContext(
            IConventionHostBuilder builder,
            IWebAssemblyHostBuilder hostBuilder,
            ILogger logger
        ) : base(logger, builder.ServiceProperties)
        {
            _builder = builder;
            _hostBuilder = hostBuilder;
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <value>The builder.</value>
        public IWebAssemblyHostBuilder Builder => _hostBuilder;

        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        public IAssemblyProvider AssemblyProvider => _builder.AssemblyProvider;

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        public IAssemblyCandidateFinder AssemblyCandidateFinder => _builder.AssemblyCandidateFinder;

        public IConventionScanner Scanner => _builder.Scanner;

        public IServiceProviderDictionary ServiceProperties => _builder.ServiceProperties;

        public IConventionHostBuilder AppendConvention(IEnumerable<IConvention> conventions)
            => _builder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention(params IConvention[] conventions)
            => _builder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention(IEnumerable<Type> conventions)
            => _builder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention(params Type[] conventions)
            => _builder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention<T>()
            where T : IConvention => _builder.AppendConvention<T>();

        public IConventionHostBuilder AppendDelegate(IEnumerable<Delegate> delegates)
            => _builder.AppendDelegate(delegates);

        public IConventionHostBuilder AppendDelegate(params Delegate[] delegates) => _builder.AppendDelegate(delegates);

        public IConventionHostBuilder PrependConvention(IEnumerable<IConvention> conventions)
            => _builder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention(params IConvention[] conventions)
            => _builder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention(IEnumerable<Type> conventions)
            => _builder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention(params Type[] conventions)
            => _builder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention<T>()
            where T : IConvention => _builder.PrependConvention<T>();

        public IConventionHostBuilder PrependDelegate(IEnumerable<Delegate> delegates)
            => _builder.PrependDelegate(delegates);

        public IConventionHostBuilder PrependDelegate(params Delegate[] delegates)
            => _builder.PrependDelegate(delegates);
    }
}