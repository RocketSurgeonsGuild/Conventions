# Creating a custom convention

The essence of a convention is a method, with any number of parameters, that can be implemented to do some work.

Lets say you want to create a convention for handling database configuration.

Goal: Distribute database configuration across assemblies

Given this fake interface for configuring the database.

[!code-c#[IDatabaseConfigurator](../../sample/Sample.Core/Databases/IDatabaseConfigurator.cs?name=codeblock)]

## Define your interface and optional delegate

Now we will define our interface and delegate. The delegate and extenison methods are optional but makes it easier for consumers to create adhoc conventions.

[!code-c#[IDatabaseConfigurator](../../sample/Sample.Core/Databases/DatabaseConvention.cs?name=codeblock)]
[!code-c#[IDatabaseConfigurator](../../sample/Sample.Core/Databases/IDatabaseConvention.cs?name=codeblock)]
[!code-c#[IDatabaseConfigurator](../../sample/Sample.Core/Databases/DatabaseConventionContextBuilderExtensions.cs?name=codeblock)]

## Create your application method

We have defined our conventions now we need to be able to apply our conventions.

There are two ways to do this, if for example your database configuration happens during service registration (perhaps via `IOptions`) then you can implement this
a convention that will run during the services convention. The other way additional way is to create an extension method that takes takes the convention and applies it.

In this example we'll support both. The manual way using an extension method and a convention that will do it "automagically".

[!code-c#[IDatabaseConfigurator](../../sample/Sample.Core/Databases/DatabaseConfiguratorExtensions.cs?name=codeblock)]
[!code-c#[IDatabaseConfigurator](../../sample/Sample.Core/Databases/DatabaseServiceConvention.cs?name=codeblock)]
