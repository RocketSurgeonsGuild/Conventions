using System.Reflection;
using System.Runtime.Serialization;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0058 // Expression value is never used

namespace Rocket.Surgery.Conventions.CommandLine;

/// <summary>
///     Uses an instance of <see cref="IServiceProvider" /> to call constructors
///     when creating models.
///     Implements the <see cref="McMaster.Extensions.CommandLineUtils.Conventions.IConvention" />
/// </summary>
/// <seealso cref="McMaster.Extensions.CommandLineUtils.Conventions.IConvention" />
public class ActivatorUtilitiesConvention : McMaster.Extensions.CommandLineUtils.Conventions.IConvention
{
    /// <summary>
    ///     The ambiguous on execute method
    /// </summary>
    public const string AmbiguousOnExecuteMethod =
        "Could not determine which 'OnExecute' or 'OnExecuteAsync' method to use. Multiple methods with this name were found";

    /// <summary>
    ///     The no on execute method found
    /// </summary>
    public const string NoOnExecuteMethodFound = "No method named 'OnExecute' or 'OnExecuteAsync' could be found";

    /// <summary>
    ///     Invalids the type of the on execute return.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>System.String.</returns>
    public static string InvalidOnExecuteReturnType(string methodName)
    {
        return methodName +
               " must have a return type of int or void, or if the method is async, Task<int> or Task.";
    }

    /// <summary>
    ///     The additional services property
    /// </summary>
    internal static readonly PropertyInfo AdditionalServicesProperty =
        typeof(CommandLineApplication)
           .GetRuntimeProperties()
           .Single(m => m.Name == "AdditionalServices");

    private static void CallConstructor(
        IServiceProvider provider,
        ConstructorInfo constructorInfo,
        object? instance
    )
    {
        var methodParams = constructorInfo.GetParameters();
        var arguments = new object[methodParams.Length];
        for (var index = 0; index < methodParams.Length; index++)
        {
            // does not support things like nullable properties
            arguments[index] = provider.GetRequiredService(methodParams[index].ParameterType);
        }

        constructorInfo.Invoke(instance, arguments);
    }

    private static readonly MethodInfo BindParametersMethod = typeof(McMaster.Extensions.CommandLineUtils.Conventions.ConventionContext).Assembly
       .GetType("McMaster.Extensions.CommandLineUtils.ReflectionHelper")!
       .GetMethod("BindParameters", BindingFlags.Public | BindingFlags.Static)!;

    private static readonly MethodInfo ApplyMethod =
        typeof(ActivatorUtilitiesConvention)
           .GetRuntimeMethods()
           .Single(m => m.Name == nameof(ApplyImpl));

    private static async Task<int> InvokeAsync(MethodInfo method, object? instance, object[] arguments)
    {
        var result = (Task)method.Invoke(instance, arguments)!;
        if (result is Task<int> intResult)
        {
            return await intResult.ConfigureAwait(false);
        }

        await result.ConfigureAwait(false);
        return 0;
    }

    private static int Invoke(MethodInfo method, object? instance, object[] arguments)
    {
        var result = method.Invoke(instance, arguments);
        if (method.ReturnType == typeof(int))
        {
            return (int)result!;
        }

        return 0;
    }

    private static void ApplyImpl<TModel>(McMaster.Extensions.CommandLineUtils.Conventions.ConventionContext context)
        where TModel : class
    {
        if (context.Application is CommandLineApplication<TModel> app)
        {
            app.ModelFactory = () => (TModel)FormatterServices.GetUninitializedObject(typeof(TModel));
        }
    }

    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivatorUtilitiesConvention" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public ActivatorUtilitiesConvention(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private static async Task<int> OnExecute(McMaster.Extensions.CommandLineUtils.Conventions.ConventionContext context, CancellationToken cancellation)
    {
        const BindingFlags binding = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        var typeInfo = context.ModelType!.GetTypeInfo();
        MethodInfo? method;
        MethodInfo? asyncMethod;
        try
        {
            method = typeInfo.GetMethod("OnExecute", binding);
            asyncMethod = typeInfo.GetMethod("OnExecuteAsync", binding);
        }
        catch (AmbiguousMatchException ex)
        {
            throw new InvalidOperationException(AmbiguousOnExecuteMethod, ex);
        }

        if (method != null && asyncMethod != null)
        {
            throw new InvalidOperationException(AmbiguousOnExecuteMethod);
        }

        method ??= asyncMethod;

        if (method == null)
        {
            throw new InvalidOperationException(NoOnExecuteMethodFound);
        }

        var constructor =
            context.ModelType!.GetTypeInfo()
                   .DeclaredConstructors.Single();
        var model = context.ModelAccessor?.GetModel();

        // Preserve any values that have been set by the command line
        var properties = context.ModelType?
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                .Where(x => x.CanRead && x.CanWrite)
                                .Select(x => ( PropertyInfo: x, value: x.GetValue(model) ))
                                .Where(x => x.value != null)
#pragma warning disable CS8619
                                .ToArray() ?? Array.Empty<(PropertyInfo, object)>(); // Lazy evaluation is killer
#pragma warning restore CS8619
        var fields = context.ModelType
                           ?.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Select(x => ( FieldInfo: x, value: x.GetValue(model) ))
                            .Where(x => x.value != null)
#pragma warning disable CS8619
                            .ToArray() ?? Array.Empty<(FieldInfo, object)>(); // Lazy evaluation is killer
#pragma warning restore CS8619

        CallConstructor(context.Application, constructor, model);

        // Restore fields that might have been overwritten
        foreach (var field in fields)
        {
            field.FieldInfo.SetValue(model, field.value);
        }

        foreach (var property in properties)
        {
            property.PropertyInfo.SetValue(model, property.value);
        }

        var arguments = (object[])BindParametersMethod.Invoke(
            null,
            new object[] { method, context.Application, cancellation }
        )!;

        if (method.ReturnType == typeof(Task) || method.ReturnType == typeof(Task<int>))
        {
            return await InvokeAsync(method, model, arguments).ConfigureAwait(false);
        }

        if (method.ReturnType == typeof(void) || method.ReturnType == typeof(int))
        {
#pragma warning disable CA1849
            // ReSharper disable once MethodHasAsyncOverload
            return Invoke(method, model, arguments);
#pragma warning restore CA1849
        }

        throw new InvalidOperationException(InvalidOnExecuteReturnType(method.Name));
    }

    /// <summary>
    ///     Apply the convention.
    /// </summary>
    /// <param name="context">The context in which the convention is applied.</param>
    /// <inheritdoc />
    public virtual void Apply(McMaster.Extensions.CommandLineUtils.Conventions.ConventionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        AdditionalServicesProperty.SetValue(context.Application, _serviceProvider);

        if (context.ModelType == null)
        {
            return;
        }

        ApplyMethod.MakeGenericMethod(context.ModelType).Invoke(this, new object[] { context });

        context.Application.OnExecuteAsync(cancellation => OnExecute(context, cancellation));
    }
}
