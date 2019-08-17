using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Diagnostics;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.CommandLine;
using Terminal.Gui;
using TerminalAttribute = Terminal.Gui.Attribute;

[assembly: Convention(typeof(ConventionCommandConvention))]

namespace Rocket.Surgery.Conventions.Diagnostics
{
    [UsedImplicitly]
    [Command("list", Description = "Applies all outstanding changes to the database based on the current configuration")]
    class ConventionListCommand
    {
        private readonly IConventionScanner scanner;
        private readonly IAssemblyCandidateFinder assemblyCandidateFinder;
        private readonly ILogger<ConventionListCommand> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionListCommand" /> class.
        /// </summary>
        public ConventionListCommand(IConventionScanner scanner, IAssemblyCandidateFinder assemblyCandidateFinder, ILogger<ConventionListCommand> logger)
        {
            this.scanner = scanner;
            this.assemblyCandidateFinder = assemblyCandidateFinder;
            this.logger = logger;
        }

        /// <summary>
        /// Called when [execute].
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int OnExecute()
        {
            Application.Init();

            var top = new CustomWindow()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            top.ColorScheme = new ColorScheme()
            {
                Focus = TerminalAttribute.Make(Color.Brown, Color.Black),
                HotFocus = TerminalAttribute.Make(Color.BrightYellow, Color.Black),
                HotNormal = TerminalAttribute.Make(Color.Gray, Color.Black),
                Normal = TerminalAttribute.Make(Color.White, Color.Black),
            };

            var discovery = CreateDiscoveryWindow();
            discovery.ColorScheme = top.ColorScheme;

            var locate = CreateLocateWindow();
            locate.ColorScheme = top.ColorScheme;

            var menu = new MenuBar(new MenuBarItem[]
            {
                new MenuBarItem ("_File", new MenuItem []
                {
                    new MenuItem ("_Discovery", "Discover registered conventions and their orders", OpenDiscovery),
                    new MenuItem ("_Locate", "Locate all available conventions from different assemblies", OpenLocate),
                })
            });

            top.Add(menu);

            void OpenDiscovery()
            {
                top.Remove(locate);
                top.Add(discovery);
                Application.RequestStop();
                Application.Run(top);
            }

            void OpenLocate()
            {
                top.Remove(discovery);
                top.Add(locate);
                Application.RequestStop();
                Application.Run(top);
            }

            OpenDiscovery();

            return 0;
        }

        private DiscoveryWindow CreateDiscoveryWindow()
        {
            var builders = assemblyCandidateFinder.GetCandidateAssemblies("Rocket.Surgery.Conventions.Abstractions", "Rocket.Surgery.Conventions")
                .SelectMany(x => x.DefinedTypes)
                .Where(x => x.IsInterface && x.ImplementedInterfaces.Any(z => z.IsGenericType && typeof(IConventionContainer<,,>) == z.GetGenericTypeDefinition()))
                .Distinct()
                .Select((x, i) => new ConventionDefinition(x))
                .OrderBy(x => x.Name)
                .ToArray();
            return new DiscoveryWindow(scanner).UpdateDefinitions(builders);
        }

        private LocateWindow CreateLocateWindow()
        {
            var builders = assemblyCandidateFinder.GetCandidateAssemblies("Rocket.Surgery.Conventions.Abstractions", "Rocket.Surgery.Conventions")
                .SelectMany(x => x.DefinedTypes)
                .Where(x => x.IsInterface && x.ImplementedInterfaces.Any(z => z.IsGenericType && typeof(IConventionContainer<,,>) == z.GetGenericTypeDefinition()))
                .Distinct()
                .Select((x, i) => new ConventionDefinition(x))
                .OrderBy(x => x.Name)
                .ToArray();
            var candidateAssemblies = assemblyCandidateFinder.GetCandidateAssemblies("Rocket.Surgery.Conventions.Abstractions", "Rocket.Surgery.Conventions")
                .Select(x => new AssemblyDefinition(x))
                .Where(x => x.Conventions.Any())
                .ToArray();
            return new LocateWindow().UpdateAssemblies(candidateAssemblies).UpdateDefinitions(builders);
        }

        private static (FrameView frame, ListView list) BuildListFrameView(string name, View? relativeTo = null, bool canFocus = false)
        {
            var frame = new FrameView(name)
            {
                X = relativeTo != null ? Pos.Right(relativeTo) : null,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var list = new ListView(Array.Empty<string>())
            {
                AllowsMarking = false,
                CanFocus = canFocus
            };
            frame.Add(list);

            return (frame, list);
        }

        private static (Label label, Label value) BuildLabelView(string name, View? relativeTo = null, int width = 12)
        {
            var label = new Label(name)
            {
                X = 1,
                Y = relativeTo == null ? 0 : Pos.Bottom(relativeTo),
                Height = 1,
                Width = width,
            };
            var value = new Label("")
            {
                X = Pos.Right(label),
                Y = relativeTo == null ? 0 : Pos.Bottom(relativeTo),
                Width = Dim.Fill(),
                Height = 1,
                CanFocus = false,
            };

            return (label, value);
        }

        class DiscoveryWindow : Window
        {
            private readonly (FrameView frame, ListView list) conventions;
            private readonly (FrameView frame, ListView list) discovered;
            private readonly FrameView details;
            private ConventionDefinition[] definitions = Array.Empty<ConventionDefinition>();
            private ConventionDetails[] conventionDetails = Array.Empty<ConventionDetails>();
            private readonly (Label label, Label value) kindView;
            private readonly (Label label, Label value) assemblyView;
            private readonly (Label label, Label value) typeView;
            private readonly IConventionScanner scanner;

            public DiscoveryWindow(IConventionScanner scanner) : base("Discovery")
            {
                Y = 1;
                conventions = BuildListFrameView("Conventions", canFocus: true);
                Add(conventions.frame);

                discovered = BuildListFrameView("Discovered", conventions.frame, canFocus: true);
                Add(discovered.frame);

                details = new FrameView("Details")
                {
                    X = Pos.Right(discovered.frame),
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };
                Add(details);

                kindView = BuildLabelView("Kind:");
                details.Add(kindView.label, kindView.value);
                assemblyView = BuildLabelView("Assembly:", kindView.label);
                details.Add(assemblyView.label, assemblyView.value);
                typeView = BuildLabelView("Type:", assemblyView.label);
                details.Add(typeView.label, typeView.value);

                conventions.list.SelectedChanged += UpdateConventions;
                discovered.list.SelectedChanged += UpdateDetails;
                this.scanner = scanner;
            }

            public DiscoveryWindow UpdateDefinitions(ConventionDefinition[] definitions)
            {
                this.definitions = definitions;
                conventions.frame.Width = definitions.Max(x => x.Name.Length) + 4;
                conventions.list.SetSource(definitions);
                UpdateConventions();
                return this;
            }

            void UpdateConventions()
            {
                if (details.Frame.Width == 0)
                {
                    Application.Current.SetNeedsDisplay();
                    Application.Current.Redraw(Application.Current.Bounds);
                }

                if (conventions.list.Source.Count == 0)
                {
                    UpdateDetails();
                    return;
                }

                var builder = definitions[conventions.list.SelectedItem];
                conventionDetails = scanner.BuildProvider().GetAll()
                    .Where(x => (x.Convention != null && builder.ConventionType.IsAssignableFrom(x.Convention.GetType())) || (x.Delegate != null && builder.DelegateType.IsAssignableFrom(x.Delegate?.GetType())))
                    .Select((cod, index) => new ConventionDetails(cod, index))
                    .ToArray();
                discovered.list.SetSource(conventionDetails);
                UpdateDetails();
            }

            void UpdateDetails()
            {
                if (discovered.list.Source.Count == 0)
                {
                    assemblyView.value.Text = "";
                    kindView.value.Text = "";
                    typeView.value.Text = "";
                    return;
                }

                discovered.frame.Width = conventionDetails.Max(x => x.ToString().Length) + 4;
                var detail = conventionDetails[discovered.list.SelectedItem];
                assemblyView.value.Text = MinLabelText(detail.Assembly.GetName().FullName);
                kindView.value.Text = MinLabelText(detail.Kind.ToString());
                typeView.value.Text = MinLabelText(detail.Type.FullName);
            }

            string MinLabelText(string? value)
            {
                return (value ?? string.Empty).Substring(0, details.Frame.Width <= 0 ? (value ?? string.Empty).Length : Math.Min((value ?? string.Empty).Length, details.Frame.Width - assemblyView.label.Frame.Width - 4));
            }
        }

        class LocateWindow : Window
        {
            private readonly (FrameView frame, ListView list) assembly;
            private readonly (FrameView frame, ListView list) conventions;
            private readonly FrameView details;
            private AssemblyDefinition[] assemblies = Array.Empty<AssemblyDefinition>();
            private ConventionDefinition[] definitions = Array.Empty<ConventionDefinition>();
            private AssemblyConventionDetails[] conventionDetails = Array.Empty<AssemblyConventionDetails>();
            private readonly (Label label, Label value) isPublicView;
            private readonly (Label label, Label value) hasAttributeView;
            private readonly (Label label, Label value) assemblyView;
            private readonly (Label label, Label value) typeView;
            private readonly Label conventionsLabel;
            private readonly Label conventionsValue;

            public LocateWindow() : base("Discovery")
            {
                Y = 1;
                assembly = BuildListFrameView("Assemblies", canFocus: true);
                Add(assembly.frame);

                conventions = BuildListFrameView("Conventions", assembly.frame, canFocus: true);
                Add(conventions.frame);

                details = new FrameView("Details")
                {
                    X = Pos.Right(conventions.frame),
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };
                Add(details);

                assemblyView = BuildLabelView("Assembly:");
                details.Add(assemblyView.label, assemblyView.value);

                typeView = BuildLabelView("Type:", assemblyView.label);
                details.Add(typeView.label, typeView.value);

                isPublicView = BuildLabelView("Is Public:", typeView.label);
                details.Add(isPublicView.label, isPublicView.value);

                hasAttributeView = BuildLabelView("Scanable:", isPublicView.label);
                details.Add(hasAttributeView.label, hasAttributeView.value);

                conventionsLabel = new Label("Implements Conventions:")
                {
                    X = 1,
                    Y = Pos.Bottom(hasAttributeView.label),
                    Height = 1,
                    Width = Dim.Fill(),
                };
                conventionsValue = new Label("")
                {
                    X = 2,
                    Y = Pos.Bottom(conventionsLabel),
                    Height = Dim.Fill(),
                    Width = Dim.Fill(),
                };
                details.Add(conventionsLabel, conventionsValue);

                Add(assembly.frame, conventions.frame, details);

                assembly.list.SelectedChanged += UpdateAssembly;
                conventions.list.SelectedChanged += UpdateDetails;
            }

            public LocateWindow UpdateAssemblies(AssemblyDefinition[] assemblies)
            {
                this.assemblies = assemblies;
                assembly.frame.Width = assemblies.Max(x => x.Name.Length) + 4;
                assembly.list.SetSource(assemblies);
                UpdateAssembly();
                return this;
            }

            public LocateWindow UpdateDefinitions(ConventionDefinition[] definitions)
            {
                this.definitions = definitions;
                conventions.frame.Width = definitions.Max(x => x.Name.Length) + 4;
                conventions.list.SetSource(definitions);
                UpdateAssembly();
                return this;
            }

            void UpdateAssembly()
            {
                if (assembly.list.Source.Count == 0)
                {
                    UpdateDetails();
                    return;
                }

                var builder = assemblies[assembly.list.SelectedItem];
                conventionDetails = builder.Conventions
                    .Select(assembly => new AssemblyConventionDetails(
                        assembly,
                        definitions
                            .Where(z => z.ConventionType.IsAssignableFrom(assembly))
                            .ToArray())
                    )
                    .ToArray();
                conventions.list.SetSource(conventionDetails);
                UpdateDetails();
            }

            void UpdateDetails()
            {
                if (conventions.list.Source.Count == 0)
                {
                    assemblyView.value.Text = "";
                    isPublicView.value.Text = "";
                    typeView.value.Text = "";
                    hasAttributeView.value.Text = "";
                    return;
                }

                conventions.frame.Width = conventionDetails.Max(x => x.ToString().Length) + 4;
                var detail = conventionDetails[conventions.list.SelectedItem];
                assemblyView.value.Text = MinLabelText(detail.Assembly.GetName().FullName);
                isPublicView.value.Text = MinLabelText(detail.IsPublic ? "Yes" : "No");
                hasAttributeView.value.Text = MinLabelText(detail.HasAttribute ? "Yes" : "No");
                typeView.value.Text = MinLabelText(detail.Type.FullName);
                conventionsValue.Text = string.Join("\n", detail.Definitions.Select(z => "* " + z.Name));
            }

            string MinLabelText(string? value)
            {
                return (value ?? string.Empty).Substring(0, details.Frame.Width <= 0 ? (value ?? string.Empty).Length : Math.Min((value ?? string.Empty).Length, details.Frame.Width - assemblyView.label.Frame.Width - 4));
            }
        }

        private class CustomWindow : Toplevel
        {
            public override bool ProcessKey(KeyEvent keyEvent)
            {
                if (keyEvent.Key == Key.Esc)
                {
                    Application.RequestStop();
                    return true;
                }

                return base.ProcessKey(keyEvent);
            }
        }
    }

    enum ConventionKind
    {
        Convention,
        Delegate
    }

    class AssemblyDefinition
    {
        public AssemblyDefinition(Assembly assembly)
        {
            Assembly = assembly;

            Conventions = assembly.DefinedTypes.Where(x => typeof(IConvention).IsAssignableFrom(x) && x.IsClass).ToArray();

        }

        public Assembly Assembly { get; }
        public string Name => Assembly.GetName()?.Name ?? string.Empty;
        public TypeInfo[] Conventions { get; }

        public override string ToString()
        {
            return Name;
        }
    }

    class AssemblyConventionDetails
    {
        public AssemblyConventionDetails(TypeInfo convention, ConventionDefinition[] definitions)
        {
            Type = convention;
            Definitions = definitions;
            HasAttribute = convention.Assembly.GetCustomAttributes<ConventionAttribute>().Any(x => x.Type.GetTypeInfo() == convention);
            IsPublic = convention.IsPublic;
        }

        public TypeInfo Type { get; }
        public ConventionDefinition[] Definitions { get; }

        public Assembly Assembly => Type.Assembly;
        public bool HasAttribute { get; }
        public bool IsPublic { get; }

        public override string ToString()
        {
            return Type.Name;
        }
    }

    class ConventionDetails
    {
        public ConventionDetails(DelegateOrConvention convention, int index)
        {
            Index = index;
            Kind = convention.Convention == null ? ConventionKind.Delegate : ConventionKind.Convention;
            if (convention.Convention != null)
            {
                Type = convention.Convention.GetType().GetTypeInfo();
                Assembly = Type.Assembly;
                Name = Type.Name;
            }
            else
            {
                Type = convention.Delegate!.Method.DeclaringType!.GetTypeInfo();
                Assembly = Type.Assembly;
                var name = convention.Delegate!.Method.Name;
                var methodType = convention.Delegate.Method.DeclaringType;
                var rootName = methodType!.FullName;

                if (methodType.IsNested)
                {
                    rootName = methodType.DeclaringType!.Name;
                }

                var regex = new Regex("<(\\w+?)>b_+\\d+?_+(\\d+?)", RegexOptions.Compiled);
                var result = regex.Match(name);
                if (result.Success && result.Groups.Count > 1)
                {
                    name = result.Groups[1].Value + "(" + result.Groups[2].Value + ")";
                }

                Name = $"{rootName}+{name}";
            }
        }

        public ConventionKind Kind { get; }
        public Assembly Assembly { get; }
        public TypeInfo Type { get; }
        public string Name { get; }
        public int Index { get; }

        public override string ToString() => $"{Index + 1}) {Name}";
    }

    class ConventionDefinition
    {
        public ConventionDefinition(TypeInfo type)
        {
            Type = type;
            var name = type.Name.TrimStart('I');
            if (name.EndsWith("Builder"))
            {
                name = name.Substring(0, name.LastIndexOf("Builder"));
            }
            if (name.EndsWith("Convention"))
            {
                name = name.Substring(0, name.LastIndexOf("Convention"));
            }
            Name = name;

            var container = type.ImplementedInterfaces.First(z => z.IsGenericType && typeof(IConventionContainer<,,>) == z.GetGenericTypeDefinition());
            ConventionType = container.GenericTypeArguments[1].GetTypeInfo();
            DelegateType = container.GenericTypeArguments[2].GetTypeInfo();
        }

        public TypeInfo Type { get; }
        public string Name { get; }
        public TypeInfo ConventionType { get; }
        public TypeInfo DelegateType { get; }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// DumpCommand.
    /// </summary>
    [UsedImplicitly]
    [Command("conventions", "convention", Description = "Convention Diagnostics")]
    [Subcommand(typeof(ConventionListCommand))]
    class ConventionCommand
    {
        private readonly CommandLineApplication _application;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionCommand" /> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public ConventionCommand(CommandLineApplication application)
        {
            _application = application;
        }

        /// <summary>
        /// Called when [execute].
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int OnExecute()
        {
            _application.ShowHelp();
            return 0;
        }
    }

    /// <summary>
    /// A common diagnostics command to attach multiple commands to.
    /// </summary>
    [Command("diagnostics", "diag", Description = "Convention Diagnostics")]
    public class DiagnosticsCommand
    {
        private readonly CommandLineApplication _application;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsCommand" /> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public DiagnosticsCommand(CommandLineApplication application)
        {
            _application = application;
        }

        /// <summary>
        /// Called when [execute].
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int OnExecute()
        {
            _application.ShowHelp();
            return 0;
        }
    }

    class ConventionCommandConvention : ICommandLineConvention
    {
        public void Register(ICommandLineConventionContext context)
        {
            var command = context.AddCommand<DiagnosticsCommand>();
            command.Command<ConventionCommand>("conventions", app => { });
        }
    }
}
