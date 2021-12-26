#if NET6_0_OR_GREATER
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.Web.Hosting;

sealed class HostingListener : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object?>>
{
    private readonly WebApplicationBuilder _webApplicationBuilder;
    private readonly ConventionContextBuilder _conventionContextBuilder;
    private IDisposable? _disposable;
    private static readonly AsyncLocal<HostingListener> _currentListener = new();
    private readonly IDisposable? _subscription;

    public static void Attach(WebApplicationBuilder webApplicationBuilder, ConventionContextBuilder conventionContextBuilder) => new HostingListener(webApplicationBuilder, conventionContextBuilder);

    internal HostingListener(WebApplicationBuilder webApplicationBuilder, ConventionContextBuilder conventionContextBuilder)
    {
        _currentListener.Value = this;
        _webApplicationBuilder = webApplicationBuilder;
        _conventionContextBuilder = conventionContextBuilder;
        _subscription = DiagnosticListener.AllListeners.Subscribe(this);
    }

    public void OnCompleted()
    {
        _disposable?.Dispose();
        _subscription?.Dispose();
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(DiagnosticListener value)
    {
        if (_currentListener.Value != this)
        {
            // Ignore events that aren't for this listener
            return;
        }

        if (value.Name == "Microsoft.Extensions.Hosting")
        {
            _disposable = value.Subscribe(this);
        }
    }

    public void OnNext(KeyValuePair<string, object?> value)
    {
        if (_currentListener.Value != this)
        {
            // Ignore events that aren't for this listener
            return;
        }

        if (value.Key == "HostBuilding" && value.Value is IHostBuilder builder)
        {
            _conventionContextBuilder.Properties.AddIfMissing(builder).AddIfMissing(HostType.Live);
            var host = new RocketContext(_webApplicationBuilder, ConventionContext.From(_conventionContextBuilder));
            host.ComposeHostingConvention();
            host.ConfigureAppConfiguration();
            host.ConfigureServices();
        }

        if (value.Key == "HostBuilt")
        {
            OnCompleted();
        }
    }
}
#endif
