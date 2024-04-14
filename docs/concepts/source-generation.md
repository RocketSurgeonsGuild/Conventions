# Source Generation and Conventions
We have built a Source Generator to use with Conventions that allows us to export conventions and import conventions with easy, in a way that is statically compiled.

# Exporting Conventions
Any convention marked with `[assembly: Convention(typeof(MyConvention))]` will automatically be added to a public partial static class named `Exports` that will exist in a namespace `Conventions` of the assembly being built.
> [!TIP]
> This class is partial if you want to add any custom methods to it for consumers.

# Importing Conventions
You can import all the conventions for a given assembly by adding the `[assembly: ImportCoventions]` attribute which will create a `Imports` class inside the `Conventions` namespace.
> [!TIP]
> This class is partial if you want to add any custom methods to it for consumers.

You can also import the `GetConventions` method onto a given class by using the `[ImportConventions]` attribute on the class.