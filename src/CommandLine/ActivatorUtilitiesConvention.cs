using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Extensions.CommandLine
{
    /// <summary>
    /// Uses an instance of <see cref="IServiceProvider" /> to call constructors
    /// when creating models.
    /// Implements the <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="IConvention" />
    public class ActivatorUtilitiesConvention : IConvention
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatorUtilitiesConvention"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ActivatorUtilitiesConvention(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Apply the convention.
        /// </summary>
        /// <param name="context">The context in which the convention is applied.</param>
        /// <inheritdoc />
        public virtual void Apply(ConventionContext context)
        {
            if (_serviceProvider != null)
            {
                AdditionalServicesProperty.SetValue(context.Application, _serviceProvider);
            }

            if (context.ModelType == null)
            {
                return;
            }

            ApplyMethod.MakeGenericMethod(context.ModelType).Invoke(this, new object[] { context });

            context.Application.OnExecuteAsync((cancellation) => OnExecute(context, cancellation));
        }

        private async Task<int> OnExecute(ConventionContext context, CancellationToken cancellation)
        {
            const BindingFlags binding = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var typeInfo = context.ModelType.GetTypeInfo();
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

            method = method ?? asyncMethod;

            if (method == null)
            {
                throw new InvalidOperationException(NoOnExecuteMethodFound);
            }

            var constructor =
                context.ModelType.GetTypeInfo()
                    .DeclaredConstructors.Single();
            var model = context.ModelAccessor?.GetModel();

            // Preserve any values that have been set by the command line
            var properties = context.ModelType?
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.CanRead && x.CanWrite)
                .Select(x => (PropertyInfo: x, value: x.GetValue(model)))
                .Where(x => x.value != null)
                .ToArray() ?? Array.Empty<(PropertyInfo, object)>(); // Lazy evaluation is killer
            var fields = context.ModelType?.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(x => (FieldInfo: x, value: x.GetValue(model)))
                .Where(x => x.value != null)
                .ToArray() ?? Array.Empty<(FieldInfo, object)>(); // Lazy evaluation is killer

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

            var arguments = (object[])BindParametersMethod.Invoke(null, new object[] { method, context.Application, cancellation })!;

            if (method.ReturnType == typeof(Task) || method.ReturnType == typeof(Task<int>))
            {
                return await InvokeAsync(method, model, arguments);
            }
            if (method.ReturnType == typeof(void) || method.ReturnType == typeof(int))
            {
                return Invoke(method, model, arguments);
            }

            throw new InvalidOperationException(InvalidOnExecuteReturnType(method.Name));
        }

        private static void CallConstructor(IServiceProvider provider, ConstructorInfo constructorInfo, object? instance)
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

        private async Task<int> InvokeAsync(MethodInfo method, object? instance, object[] arguments)
        {
            var result = (Task)method.Invoke(instance, arguments)!;
            if (result is Task<int> intResult)
            {
                return await intResult;
            }

            await result;
            return 0;
        }

        private int Invoke(MethodInfo method, object? instance, object[] arguments)
        {
            var result = method.Invoke(instance, arguments);
            if (method.ReturnType == typeof(int))
            {
                return (int)result!;
            }

            return 0;
        }



        private static readonly MethodInfo BindParametersMethod = typeof(ConventionContext).Assembly
            .GetType("McMaster.Extensions.CommandLineUtils.ReflectionHelper")!
            .GetMethod("BindParameters", BindingFlags.Public | BindingFlags.Static)!;

        /// <summary>
        /// The ambiguous on execute method
        /// </summary>
        public const string AmbiguousOnExecuteMethod = "Could not determine which 'OnExecute' or 'OnExecuteAsync' method to use. Multiple methods with this name were found";

        /// <summary>
        /// The no on execute method found
        /// </summary>
        public const string NoOnExecuteMethodFound = "No method named 'OnExecute' or 'OnExecuteAsync' could be found";

        /// <summary>
        /// Invalids the type of the on execute return.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>System.String.</returns>
        public static string InvalidOnExecuteReturnType(string methodName) => methodName + " must have a return type of int or void, or if the method is async, Task<int> or Task.";

        /// <summary>
        /// The additional services property
        /// </summary>
        internal static readonly PropertyInfo AdditionalServicesProperty =
            typeof(CommandLineApplication)
                .GetRuntimeProperties()
                .Single(m => m.Name == "AdditionalServices");

        private static readonly MethodInfo ApplyMethod =
            typeof(ActivatorUtilitiesConvention)
                .GetRuntimeMethods()
                .Single(m => m.Name == nameof(ApplyImpl));

        private void ApplyImpl<TModel>(ConventionContext context)
            where TModel : class
        {
            if (context.Application is CommandLineApplication<TModel> app)
                app.ModelFactory = () => (TModel)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(TModel));
        }
    }
}
