# Defining Conventions
As stated [previously](./defining-conventions.md) conventions are nothing more than a class that implements <xref:Rocket.Surgery.Conventions.IConvention>.

## Implementing Multiple Conventions
A defined convention can implement as many conventions as you want.  Configuration, Services, Command Line, Custom Conventions can all be be handled by one convention.

```c#
public class MultipleConvention : IServiceConvention, IConfigurationConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services) { }
    public void Register(IConventionContext context, IConfiguration configuration, IConfigurationBuilder builder) { }
}
```

## Convention Ordering
By default the order of conventions is [non-deterministic](https://en.m.wikipedia.org/wiki/Nondeterministic_algorithm) and not controlled. Conventions are "left in place" such that once the list of conventions is created it is no changed between convention runs.  

Sometimes you want to ensure that one convention runs before another, that can be done using different attributes.  Delegate 
conventions _cannot_ be ordered in this manner and must be sorted manually.

> [!NOTE]
> Sorting is done using a [Topological sort](https://en.wikipedia.org/wiki/Topological_sorting) and if you define a cycle (A requires B, B requires A)
then a `NotSupportedException` will be thrown.


### [[BeforeConvention]](xref:Rocket.Surgery.Conventions.BeforeConventionAttribute) / [[DependentOfConvention]](xref:Rocket.Surgery.Conventions.DependentOfConventionAttribute)
This attribute can be used to ensure that your convention is called before another convention.  The order is still non-deterministic but
Conventions will ensure that the defined convention is run before the convention type defined by the attribute.

> [!NOTE]
> Multiple dependencies can be defined.

```c#
// A before B
[BeforeConvention(typeof(ConventionB))]
public class ConventionA
{
}

public class ConventionB
{
}
```


### [[AfterConvention]](xref:Rocket.Surgery.Conventions.AfterConventionAttribute) / [[DependsOnConvention]](xref:Rocket.Surgery.Conventions.DependsOnConventionAttribute)
This attribute can be used to ensure that your convention is called after another convention.  The order is still non-deterministic but
Conventions will ensure that the defined convention is run after the convention type defined by the attribute.

> [!NOTE]
> Multiple dependencies can be defined.

```c#
// B before A
[AfterConvention(typeof(ConventionB))]
public class ConventionA
{
}

public class ConventionB
{
}
```

### Custom Attributes
If you want you can define your own custom attribute for use with convention ordering.  Your attribute must implement <xref:Rocket.Surgery.Conventions.IConventionDependency>
and specify the <xref:Rocket.Surgery.Conventions.DependencyDirection>.

## Convention Host Type
You can define a convention that only runs for a given <xref:Rocket.Surgery.Conventions.HostType>. The HostType can be defined on the
<xref:Rocket.Surgery.Conventions.ConventionContextBuilder> using `Set<HostType>(HostType.UnitTest)`. Using the `HostType` an assembly
can define different behaviors given the context.

A good example of this is designing a system that uses [NodaTime](https://nodatime.org/).  You can have a [[LiveConvention]](xref:Rocket.Surgery.Conventions.LiveConventionAttribute) that registers
`IClock` using the expected calendar system, and another [[UnitTestConvention]](xref:Rocket.Surgery.Conventions.UnitTestConventionAttribute) that registers a `FakeClock`
with a predefined starting date and time.

> [!NOTE]
> Currently only three host types are defined

* `Undefined` - The default host type, means the convention runs everywhere.
* `Live` - This convention applies in a live running application
* `UnitTest` - This convention applies to unit tests.

> [!NOTE]
> There is no attribute for the `HostType.Undefined`.

### [[LiveConvention]](xref:Rocket.Surgery.Conventions.LiveConventionAttribute)
This attribute ensures a convention that only runs when the `HostType` is `Live`.

```c#
[LiveConvention]
public class ConventionA
{
}
```

### [[UnitTestConvention]](xref:Rocket.Surgery.Conventions.UnitTestConventionAttribute)
This attribute ensures a convention that only runs when the `HostType` is `UnitTest`.

```c#
[UnitTestConvention]
public class ConventionA
{
}
```


## Injecting dependencies
By default conventions are a simple class that can be created using `Activator.CreateInstance` however the goal of conventions was to allow "bootstrap time" 
configuration to be provided.  The <xref:Rocket.Surgery.Conventions.ConventionContextBuilder> acts as a bag of properties using [`indexer`](xref:Rocket.Surgery.Conventions.ConventionContextBuilder#Rocket_Surgery_Conventions_ConventionContextBuilder_Item_System_Object_) or [`Properties`](xref:Rocket.Surgery.Conventions.ConventionContextBuilder#Rocket_Surgery_Conventions_ConventionContextBuilder_Properties).

Any type defined in the context is injectable into any defined convention.  This can be used for any number of purposes such as...
* allow consumers to provide configuration to your convention
* allow conventions to communicate or share common state

Under the covers the [`Properties`](xref:Rocket.Surgery.Conventions.ConventionContextBuilder#Rocket_Surgery_Conventions_ConventionContextBuilder_Properties) implements `IServiceProvider`
and we simply activate conventions using `ActivatorUtilities.CreateInstance`.

> [!NOTE]
> All dependencies must be registered into the builder, as activation happens while <xref:Rocket.Surgery.Conventions.IConventionContext> is built.

---

```c#
public class InjectableConvention : IServiceConvention
{
    private readonly IInjectData _convention;

    public InjectableConvention(IInjectData convention) => _convention = convention;

    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        //...
    }
}

public class OptionalInjectableConvention : IServiceConvention
{
    private readonly IInjectData? _convention;

    public OptionalInjectableConvention(IInjectData? convention = null) => _convention = convention;

    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        if (_convention is { })
        {
            //...
        }
    }
}
```
