//#if NET6_0_OR_GREATER
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyModel;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Rocket.Surgery.Conventions;
//
//#pragma warning disable CA1031
//#pragma warning disable CA2000
//
//// ReSharper disable once CheckNamespace
//namespace Rocket.Surgery.Hosting;
//
///// <summary>
/////     Class RocketHostExtensions.
///// </summary>
//public static class RocketWebHostExtensions
//{
//    /// <summary>
//    ///     Configures the rocket Surgery.
//    /// </summary>
//    /// <param name="builder">The builder.</param>
//    /// <returns>WebApplicationBuilder.</returns>
//    public static WebApplicationBuilder ConfigureRocketSurgery(this WebApplicationBuilder builder)
//    {
//        if (builder == null)
//        {
//            throw new ArgumentNullException(nameof(builder));
//        }
//
//        return ConfigureRocketSurgery(builder, _ => { });
//    }
//
//    /// <summary>
//    ///     Configures the rocket Surgery.
//    /// </summary>
//    /// <param name="builder">The builder.</param>
//    /// <param name="action">The action.</param>
//    /// <returns>WebApplicationBuilder.</returns>
//    public static WebApplicationBuilder ConfigureRocketSurgery(this WebApplicationBuilder builder, Action<ConventionContextBuilder> action)
//    {
//        if (builder == null)
//        {
//            throw new ArgumentNullException(nameof(builder));
//        }
//
//        if (action == null)
//        {
//            throw new ArgumentNullException(nameof(action));
//        }
//
//        builder.Host.Properties[typeof(WebApplicationBuilder)] = builder;
//        action(SetupConventions(builder));
//        return builder;
//    }
//
//    /// <summary>
//    ///     Configures the rocket Surgery.
//    /// </summary>
//    /// <param name="builder">The builder.</param>
//    /// <param name="conventionContextBuilder">The convention context builder.</param>
//    /// <returns>WebApplicationBuilder.</returns>
//    public static WebApplicationBuilder ConfigureRocketSurgery(this WebApplicationBuilder builder, ConventionContextBuilder conventionContextBuilder)
//    {
//        if (builder == null)
//        {
//            throw new ArgumentNullException(nameof(builder));
//        }
//
//        if (conventionContextBuilder == null)
//        {
//            throw new ArgumentNullException(nameof(conventionContextBuilder));
//        }
//
//        builder.Host.Properties[typeof(WebApplicationBuilder)] = builder;
//        SetupConventions(builder, conventionContextBuilder);
//        return builder;
//    }
//
//    /// <summary>
//    ///     Uses the rocket booster.
//    /// </summary>
//    /// <param name="builder">The builder.</param>
//    /// <param name="func">The function.</param>
//    /// <param name="action">The action.</param>
//    /// <returns>WebApplicationBuilder.</returns>
//    public static WebApplicationBuilder UseRocketBooster(
//        this WebApplicationBuilder builder,
//        Func<WebApplicationBuilder, ConventionContextBuilder> func,
//        Action<ConventionContextBuilder>? action = null
//    )
//    {
//        if (builder == null)
//        {
//            throw new ArgumentNullException(nameof(builder));
//        }
//
//        if (func == null)
//        {
//            throw new ArgumentNullException(nameof(func));
//        }
//
//        var b = func(builder);
//        SetupConventions(builder, b);
//        action?.Invoke(b);
//        return builder;
//    }
//
//    /// <summary>
//    ///     Launches the with.
//    /// </summary>
//    /// <param name="builder">The builder.</param>
//    /// <param name="func">The function.</param>
//    /// <param name="action">The action.</param>
//    /// <returns>WebApplicationBuilder.</returns>
//    public static WebApplicationBuilder LaunchWith(
//        this WebApplicationBuilder builder,
//        Func<WebApplicationBuilder, ConventionContextBuilder> func,
//        Action<ConventionContextBuilder>? action = null
//    )
//    {
//        if (builder == null)
//        {
//            throw new ArgumentNullException(nameof(builder));
//        }
//
//        if (func == null)
//        {
//            throw new ArgumentNullException(nameof(func));
//        }
//
//        var b = func(builder);
//        SetupConventions(builder, b);
//        action?.Invoke(b);
//        return builder;
//    }
//
//    /// <summary>
//    ///     Gets the or create builder.
//    /// </summary>
//    /// <param name="builder">The builder.</param>
//    /// <returns>RocketHostBuilder.</returns>
//    public static ConventionContextBuilder SetupConventions(WebApplicationBuilder builder)
//    {
//        if (builder.Host.Properties.ContainsKey(typeof(ConventionContextBuilder)))
//            return ( builder.Host.Properties[typeof(ConventionContextBuilder)] as ConventionContextBuilder )!;
//
//        var conventionContextBuilder = Configure(builder, new ConventionContextBuilder(builder.Host.Properties!).UseDependencyContext(DependencyContext.Default));
//        builder.Host.Properties[typeof(ConventionContextBuilder)] = conventionContextBuilder;
//        // builder.Properties[typeof(WebApplicationBuilder)] = builder;
//        return conventionContextBuilder;
//    }
//
//    /// <summary>
//    ///     Gets the or create builder.
//    /// </summary>
//    /// <param name="builder">The builder.</param>
//    /// <param name="conventionContextBuilder">The convention context builder.</param>
//    /// <returns>RocketHostBuilder.</returns>
//    public static ConventionContextBuilder SetupConventions(WebApplicationBuilder builder, ConventionContextBuilder conventionContextBuilder)
//    {
//        if (builder.Host.Properties.ContainsKey(typeof(ConventionContextBuilder)))
//            return ( builder.Host.Properties[typeof(ConventionContextBuilder)] as ConventionContextBuilder )!;
//
//        builder.Host.Properties[typeof(ConventionContextBuilder)] = conventionContextBuilder;
//        Configure(builder, conventionContextBuilder.UseDependencyContext(DependencyContext.Default));
//        // builder.Properties[typeof(WebApplicationBuilder)] = builder;
//        return conventionContextBuilder;
//    }
//
//    /// <summary>
//    ///     Gets the or create builder.
//    /// </summary>
//    /// <param name="builder">The builder.</param>
//    /// <param name="contextBuilder"></param>
//    /// <returns>RocketHostBuilder.</returns>
//    public static IConventionContext Configure(WebApplicationBuilder builder, ConventionContextBuilder contextBuilder)
//    {
//        return Configure(builder, ConventionContext.From(contextBuilder));
//    }
//
//    /// <summary>
//    ///     Gets the or create builder.
//    /// </summary>
//    /// <param name="builder">The builder.</param>
//    /// <param name="context"></param>
//    /// <returns>RocketHostBuilder.</returns>
//    public static IConventionContext Configure(WebApplicationBuilder builder, IConventionContext context)
//    {
//        var host = new RocketContext(builder, context);
//        host.ComposeHostingConvention();
//        host.ConfigureAppConfiguration();
//        host.ConfigureServices();
//        return context;
//    }
//}
//#endif
