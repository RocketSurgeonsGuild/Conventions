# Convention Contexts

The convention context is the the result of collecting all the known conventions for a given set of assembly.  How those assemblies are collected can diff
depending on on the convention context is created.  Generally the convention context is created using the <xref:Rocket.Surgery.Conventions.ConventionContextBuilder>
however you can implement your own <xref:Rocket.Surgery.Conventions.IConventionContext> if you wish.

## Builder
A convention context is created from a <xref:Rocket.Surgery.Conventions.ConventionContextBuilder>.  This class is used during the bootstrapping phase of your application.

You can add conventions manually, you can add them via attribute scanning, you can disable attribute scanning if you wish as well.  

The assemblies used during scanning can be added by using an AppDomain, DependencyContext, or List of assemblies.

## Creating the context
A context can be created from a <xref:Rocket.Surgery.Conventions.ConventionContextBuilder> by using [ConventionContext.From](xref:Rocket.Surgery.Conventions.ConventionContext#Rocket_Surgery_Conventions_ConventionContext_From_Rocket_Surgery_Conventions_ConventionContextBuilder_).

## Using the context
Once the context is created you can use the context to find out all sorts of information.

Useful properties:
* [`AssemblyCandidateFinder`](xref:Rocket.Surgery.Conventions.IConventionContext#Rocket_Surgery_Conventions_ConventionContext_AssemblyCandidateFinder) - The assembly candidate finder can be used to intelligently find candidate assemblies based on those provided to the builder.
* [`AssemblyProvider`](xref:Rocket.Surgery.Conventions.IConventionContext#Rocket_Surgery_Conventions_ConventionContext_AssemblyProvider) - The assembly provider can be used to get a list of assemblies
* [`Logger`](xref:Rocket.Surgery.Conventions.IConventionContext#Rocket_Surgery_Conventions_ConventionContext_Logger) - This is a diagnostic logger that can be used for logging details.  If a logger is provided to the builder it will be used here.
* [`Properties`](xref:Rocket.Surgery.Conventions.IConventionContext#Rocket_Surgery_Conventions_ConventionContext_Properties) - Contains all the properties provided to the builder.  This implements `IServiceProvider` and can be used with `ActivatorExtensions

Useful methods / extension methods:
* [`Get<T>([string name])`](xref:Rocket.Surgery.Conventions.ConventionContextExtensions#Rocket_Surgery_Conventions_ConventionContextExtensions_Get__1_Rocket_Surgery_Conventions_IConventionContext_) - Get a given type from the `Properties` dictionary.
* [`GetHostType()`](xref:Rocket.Surgery.Conventions.ConventionContextExtensions#Rocket_Surgery_Conventions_ConventionContextExtensions_GetHostType_Rocket_Surgery_Conventions_IConventionContext_) - Get's the given host type, as defined in the builder. 
* [`IsUnitTestHost()`](xref:Rocket.Surgery.Conventions.ConventionContextExtensions#Rocket_Surgery_Conventions_ConventionContextExtensions_IsUnitTestHost_Rocket_Surgery_Conventions_IConventionContext_) - Tests if the builder was setup for unit testing
  * This is handy to ensure different behavior during [unit tests](./unit-tests.md).
    