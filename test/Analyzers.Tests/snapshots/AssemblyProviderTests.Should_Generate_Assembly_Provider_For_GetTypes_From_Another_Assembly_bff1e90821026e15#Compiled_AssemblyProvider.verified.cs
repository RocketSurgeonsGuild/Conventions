//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Compiled_AssemblyProvider.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace TestProject.Conventions
{
    internal static partial class Imports
    {
        private class AssemblyProvider(AssemblyLoadContext context) : IAssemblyProvider
        {
            IEnumerable<Assembly> IAssemblyProvider.GetAssemblies(Action<IAssemblyProviderAssemblySelector> action, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    case 202:
                        yield return typeof(global::TestConvention).Assembly;
                        yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions).Assembly;
                        yield return typeof(global::Dep1.Dep1Exports).Assembly;
                        yield return typeof(global::Sample.DependencyThree.Class3).Assembly;
                        yield return typeof(global::Dep2Exports).Assembly;
                        yield return context.LoadFromAssemblyName(TestProjectVersion0000CultureneutralPublicKeyTokennull);
                        break;
                }
            }

            IEnumerable<Type> IAssemblyProvider.GetTypes(Func<ITypeProviderAssemblySelector, IEnumerable<Type>> selector, string filePath, string memberName, int lineNumber)
            {
                switch (lineNumber)
                {
                    case 14:
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("EnumPolyfill");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspChildControlTypeAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspChildControlTypeAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspDataFieldAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspDataFieldAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspDataFieldsAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspDataFieldsAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMethodPropertyAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMethodPropertyAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcActionAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcActionAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcActionSelectorAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcActionSelectorAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcAreaAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcAreaAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcAreaMasterLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcAreaMasterLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcAreaPartialViewLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcAreaPartialViewLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcAreaViewLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcAreaViewLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcControllerAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcControllerAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcDisplayTemplateAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcDisplayTemplateAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcEditorTemplateAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcEditorTemplateAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcMasterAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcMasterAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcMasterLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcMasterLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcModelTypeAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcModelTypeAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcPartialViewAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcPartialViewAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcPartialViewLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcPartialViewLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcSuppressViewErrorAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcSuppressViewErrorAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcTemplateAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcTemplateAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcViewAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcViewAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcViewComponentAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcViewComponentAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcViewComponentViewAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcViewComponentViewAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcViewLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspMvcViewLocationFormatAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspRequiredAttributeAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspRequiredAttributeAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspTypePropertyAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AspTypePropertyAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AssertionConditionAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AssertionConditionAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AssertionConditionType");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AssertionConditionType");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AssertionMethodAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.AssertionMethodAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.BaseTypeRequiredAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.BaseTypeRequiredAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.CanBeNullAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.CanBeNullAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.CannotApplyEqualityOperatorAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.CannotApplyEqualityOperatorAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.CollectionAccessAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.CollectionAccessAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.CollectionAccessType");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.CollectionAccessType");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ContractAnnotationAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ContractAnnotationAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.HtmlAttributeValueAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.HtmlAttributeValueAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.HtmlElementAttributesAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.HtmlElementAttributesAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ImplicitUseKindFlags");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ImplicitUseKindFlags");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ImplicitUseTargetFlags");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ImplicitUseTargetFlags");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.InstantHandleAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.InstantHandleAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.InvokerParameterNameAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.InvokerParameterNameAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ItemCanBeNullAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ItemCanBeNullAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ItemNotNullAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ItemNotNullAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.LinqTunnelAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.LinqTunnelAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.LocalizationRequiredAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.LocalizationRequiredAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.MacroAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.MacroAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.MeansImplicitUseAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.MeansImplicitUseAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.MustUseReturnValueAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.MustUseReturnValueAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.NoEnumerationAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.NoEnumerationAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.NoReorderAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.NoReorderAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.NotifyPropertyChangedInvocatorAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.NotifyPropertyChangedInvocatorAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.NotNullAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.NotNullAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.PathReferenceAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.PathReferenceAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ProvidesContextAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ProvidesContextAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.PublicAPIAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.PublicAPIAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.PureAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.PureAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorDirectiveAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorDirectiveAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorHelperCommonAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorHelperCommonAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorImportNamespaceAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorImportNamespaceAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorInjectionAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorInjectionAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorLayoutAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorLayoutAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorPageBaseTypeAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorPageBaseTypeAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorSectionAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorSectionAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorWriteLiteralMethodAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorWriteLiteralMethodAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorWriteMethodAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorWriteMethodAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorWriteMethodParameterAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RazorWriteMethodParameterAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RegexPatternAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.RegexPatternAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.SourceTemplateAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.SourceTemplateAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.StringFormatMethodAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.StringFormatMethodAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.TerminatesProgramAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.TerminatesProgramAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.UsedImplicitlyAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.UsedImplicitlyAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ValueProviderAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.ValueProviderAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.XamlItemBindingOfItemsControlAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.XamlItemBindingOfItemsControlAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.XamlItemsControlAttribute");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("JetBrains.Annotations.XamlItemsControlAttribute");
                        yield return typeof(global::Microsoft.Extensions.Configuration.RocketSurgeryLoggingExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Microsoft.Extensions.DependencyInjection.LoggingBuilder");
                        yield return typeof(global::Microsoft.Extensions.DependencyInjection.RocketSurgeryServiceCollectionExtensions);
                        yield return typeof(global::Microsoft.Extensions.Logging.RocketSurgeryLoggingExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Polyfill");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("RegexPolyfill");
                        yield return typeof(global::Rocket.Surgery.Conventions.AbstractConventionContextBuilderExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Adapters.IServiceFactoryAdapter");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Adapters.ServiceFactoryAdapter`1");
                        yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.AfterConventionAttribute<>);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.AssemblyProviderFactory");
                        yield return typeof(global::Rocket.Surgery.Conventions.BeforeConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.BeforeConventionAttribute<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderApplicationDelegate);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderDelegateResult);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationBuilderEnvironmentDelegate);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.ConfigurationOptionsExtensions);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Configuration.IConfigurationConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionAttribute<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContext);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilder);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextBuilderExtensions);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionContextExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionContextHelpers");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionDependency");
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionHostBuilderExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionHostBuilderExtensions+ServiceProviderWrapper`1");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionOrDelegate");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ConventionProvider");
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionProviderFactory);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionsConfigurationAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.ConventionWithDependencies);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyDirection);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.IServiceConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependencyInjection.ServiceConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependentOfConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependentOfConventionAttribute<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependsOnConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.DependsOnConventionAttribute<>);
                        yield return typeof(global::Rocket.Surgery.Conventions.ExportConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.ExportConventionsAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.ExportedConventionsAttribute);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Extensions.RocketSurgerySetupExtensions");
                        yield return typeof(global::Rocket.Surgery.Conventions.HostType);
                        yield return typeof(global::Rocket.Surgery.Conventions.IAssemblyProvider);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionContext);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionDependency);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionProvider);
                        yield return typeof(global::Rocket.Surgery.Conventions.IConventionWithDependencies);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.IHostBasedConvention");
                        yield return typeof(global::Rocket.Surgery.Conventions.ImportConventionsAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.IReadOnlyServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.IServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.LiveConventionAttribute);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.ILoggingConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.LoggingConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Logging.RocketLoggingOptions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.LoggingExtensions");
                        yield return typeof(global::Rocket.Surgery.Conventions.ReadOnlyServiceProviderDictionary);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.AppDomainAssemblyProvider");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.AssemblyCandidateResolver");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.AssemblyCandidateResolver+Dependency");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.AssemblyProviderAssemblySelector");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.DefaultAssemblyProvider");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.DependencyClassification");
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.IAssemblyProviderAssemblySelector);
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeFilter);
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeProviderAssemblySelector);
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.ITypeSelector);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.TypeFilter");
                        yield return typeof(global::Rocket.Surgery.Conventions.Reflection.TypeKindFilter);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.Reflection.TypeProviderAssemblySelector");
                        yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionary);
                        yield return typeof(global::Rocket.Surgery.Conventions.ServiceProviderDictionaryExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ServiceProviderFactoryAdapter");
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.ISetupConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupAsyncConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Setup.SetupConvention);
                        yield return typeof(global::Rocket.Surgery.Conventions.Testing.TestConventionContextBuilderExtensions);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("Rocket.Surgery.Conventions.ThrowHelper");
                        yield return typeof(global::Rocket.Surgery.Conventions.UnitTestConventionAttribute);
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull).GetType("StringPolyfill");
                        yield return context.LoadFromAssemblyName(RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull).GetType("System.Runtime.CompilerServices.IsExternalInit");
                        break;
                }
            }

            private static AssemblyName _RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull;
            private static AssemblyName RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull => _RocketSurgeryConventionsVersion1000CultureneutralPublicKeyTokennull ??= new AssemblyName("Rocket.Surgery.Conventions, Version=version, Culture=neutral, PublicKeyToken=null");

            private static AssemblyName _RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull;
            private static AssemblyName RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull => _RocketSurgeryConventionsAbstractionsVersion1000CultureneutralPublicKeyTokennull ??= new AssemblyName("Rocket.Surgery.Conventions.Abstractions, Version=version, Culture=neutral, PublicKeyToken=null");

            private static AssemblyName _TestProjectVersion0000CultureneutralPublicKeyTokennull;
            private static AssemblyName TestProjectVersion0000CultureneutralPublicKeyTokennull => _TestProjectVersion0000CultureneutralPublicKeyTokennull ??= new AssemblyName("TestProject, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        }
    }
}