# What are Conventions?

The goal of Conventions is to be a simple way to bridge the gap between Configuration driven development and Convention driven development. 
In an ASP.NET Core application it is "pay-for-play" in that you must register all your configurations, services, etc.  This is great but it 
also forces you into cases where if you forget to call an extension method nothing lights up like you expected.  Another draw back of using
extension methods is that any dependencies you need, to configure services for example, must be included in the method call.

One thing to keep in mind is that ASP.NET Core just got rid of the [double container](https://github.com/aspnet/Announcements/issues/353) 
problem, we are not introducing a second container, instead we just implement `IServiceProvider` using the same [dictionary that is provided
by `IHostBuilder.Properties`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostbuilder.properties).  Any services
that you inject into your conventions must exist inside of this dictionary, or be optional.

At a high level Conventions this problem in a very similar way to something like [Autofac Modules](https://autofaccn.readthedocs.io/en/latest/configuration/modules.html) 
or other similar assembly scanning solutions.  Our goal is to be agnostic at a high level, strictly speaking using MSDI/MSEL/MSC is not required however is currently 
assumed that .NET Platform Extensions are going to be used.  In the future this might be broken out.

## Why use Conventions?
You have to make a choice with every library you introduce, and using Conventions is no different.  If it's not for you, that's okay!  Here are some possible reasons why and why not to use Conventions. 

### Reasons to use?
* You can create library code that "just works" without having to ensure that the correct extension method was called, with the corret parameters, for every library you add.
* You can worry about service registration in each convention class isolated from the rest of your application.
* You can can group bootstrapping logic together to lessen cognitive load.  
    
### Reasons not to use?
Everything has it's positives and negatives. We cannot lie using conventions will add time to starting up your application.  There is a trade off to be made here
and optimizations that can be made to lessen that trade-off.  

Fundamentally if you're making a highly focused, and specialized microservice then using Conventions may not be for you.  You will own the burden
to ensuring that configuration is done the same, extension methods are called, and everything else.  It is called "pay-to-play" after all.

#### Note on performance
A quick note on performance.  We are now using Source Generators to generate export methods, that return all conventions for a given assembly. This allows convention
loading to have similar performance characteristics to pure extension methods, it's still slower but reduces the need to assembly scan. 

# What is a Convention

A convention is nothing but a class that implements the <xref:Rocket.Surgery.Conventions.IConvention> interface.  Nothing special.
By itself <xref:Rocket.Surgery.Conventions.IConvention> is just a marker interface used to identify a class that may implement one more conventions.

Conventions are then interfaces that are implemented by the class some examples are:
* <xref:Rocket.Surgery.Conventions.Configuration.IConfigurationConvention>
* <xref:Rocket.Surgery.Conventions.DependencyInjection.IServiceConvention>
* <xref:Rocket.Surgery.Conventions.Logging.ILoggingConvention>
* <xref:Rocket.Surgery.Hosting.IHostingConvention>

ad-hoc Conventions (or delegate Conventions) are a different kind of convention but written as a delegate, but they generally mirror the method given by the Convention interface.  
These can be used to during program startup just like `ConfigureServices` are used today. 

Some examples are:
* <xref:Rocket.Surgery.Conventions.Configuration.ConfigurationConvention>
* <xref:Rocket.Surgery.Conventions.DependencyInjection.ServiceConvention>
* <xref:Rocket.Surgery.Conventions.Logging.LoggingConvention>
* <xref:Rocket.Surgery.Hosting.HostingConvention>

## Exporting a Convention

A convention can be exported by using the <xref:Rocket.Surgery.Conventions.ConventionAttribute>  (eg. `[assembly: Convention(typeof(MyConvention))]`).  
This is important you may want to make conventions are **not** enabled by default but are available to be loaded by users if they want to.

The rules are straight forward.

* A public convention - Will not be loaded unless it is added to the <xref:Rocket.Surgery.Conventions.ConventionContextBuilder>.
* A public exported convention - Will always be loaded unless excluded by assembly or type in the <xref:Rocket.Surgery.Conventions.ConventionContextBuilder>.
* A private convention - Will never be loaded. ever.
* A private exported convention - Will always be loaded unless excluded by assembly in the <xref:Rocket.Surgery.Conventions.ConventionContextBuilder>.

# Writing your own custom conventions
Creating your own conventions is fairly easily, and can be done by using existing conventions or by executing it yourself.

More information can be found [here](../guides/custom-conventions.md)

