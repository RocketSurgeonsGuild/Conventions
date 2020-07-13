using System;
using System.Collections.Generic;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Tests.DependencyInjection
{
    class DryIocBuilder : ConventionBuilder<DryIocBuilder, IServiceConvention, ServiceConventionDelegate>,
                          IServicesBuilder
    {
        private IContainer _container => this.Get<IContainer>();

        public DryIocBuilder(
            IHostEnvironment environment,
            IConfiguration configuration,
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceCollection services,
            IContainer container,
            ILogger diagnosticSource,
            IDictionary<object, object?> properties
        )
            : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));

            this.Set(container ?? throw new ArgumentNullException(nameof(container)));
        }

        public DryIocBuilder ConfigureContainer(Func<IContainer, IContainer> configureContainer)
        {
            this.Set(configureContainer?.Invoke(_container) ?? _container);
            return this;
        }

        public DryIocBuilder ConfigureContainer(Action<IContainer> configureContainer)
        {
            configureContainer?.Invoke(_container);
            return this;
        }

        public IContainer Build()
        {
            Composer.Register(
                Scanner,
                this,
                typeof(IServiceConvention),
                typeof(ServiceConventionDelegate)
            );

            _container.Populate(Services);

            return _container;
        }

        public IConfiguration Configuration { get; }
        public IServiceCollection Services { get; }
        public IObservable<IServiceProvider> OnBuild { get; }
        public IHostEnvironment Environment { get; }
        public ILogger Logger { get; }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            AppendConvention(params IServiceConvention[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            AppendConvention(IEnumerable<IServiceConvention> conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            AppendConvention<T>()
        {
            Scanner.AppendConvention<T>();
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            PrependConvention(params IServiceConvention[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            PrependConvention(IEnumerable<IServiceConvention> conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            PrependConvention<T>()
        {
            Scanner.PrependConvention<T>();
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            PrependDelegate(params ServiceConventionDelegate[] delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            PrependDelegate(IEnumerable<ServiceConventionDelegate> delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            AppendDelegate(params ServiceConventionDelegate[] delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        IServicesBuilder IConventionContainer<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>.
            AppendDelegate(IEnumerable<ServiceConventionDelegate> delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        IServiceProvider IServicesBuilder.Build() => Build();
    }
}