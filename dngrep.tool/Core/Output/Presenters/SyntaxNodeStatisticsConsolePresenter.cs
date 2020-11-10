using System;
using System.Collections.Generic;
using dngrep.tool.Abstractions.System;
using dngrep.tool.Core.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dngrep.tool.Core.Output.Presenters
{
    public class SyntaxNodeStatisticsConsolePresenter : ISyntaxNodePresenter
    {
        private readonly IConsole console;

        public SyntaxNodeStatisticsConsolePresenter(IConsole console)
        {
            this.console = console;
        }

        public void ProduceOutput(IEnumerable<SyntaxNode> nodes, GrepOptions options)
        {
            _ = nodes ?? throw new ArgumentNullException(nameof(nodes));

            var classes = 0;
            var structs = 0;
            var enums = 0;
            var namespaces = 0;
            var interfaces = 0;
            var fields = 0;
            var props = 0;
            var methods = 0;
            var args = 0;
            var localVariables = 0;

            foreach (var node in nodes)
            {
                Type nodeType = node.GetType();

                if (nodeType == typeof(ClassDeclarationSyntax))
                {
                    classes++;
                }
                else if (nodeType == typeof(StructDeclarationSyntax))
                {
                    structs++;
                }
                else if (nodeType == typeof(EnumDeclarationSyntax))
                {
                    enums++;
                }
                else if (nodeType == typeof(NamespaceDeclarationSyntax))
                {
                    namespaces++;
                }
                else if (nodeType == typeof(InterfaceDeclarationSyntax))
                {
                    interfaces++;
                }
                else if (nodeType == typeof(FieldDeclarationSyntax))
                {
                    fields++;
                }
                else if (nodeType == typeof(PropertyDeclarationSyntax))
                {
                    props++;
                }
                else if (nodeType == typeof(MethodDeclarationSyntax))
                {
                    methods++;
                }
                else if (nodeType == typeof(ArgumentSyntax))
                {
                    args++;
                }
                else if (nodeType == typeof(LocalDeclarationStatementSyntax))
                {
                    localVariables++;
                }
            }

            if (classes > 0)
            {
                this.console.WriteLine($"Classes:\t{classes}");
            }

            if (structs > 0)
            {
                this.console.WriteLine($"Structs:\t{structs}");
            }

            if (enums > 0)
            {
                this.console.WriteLine($"Enums:\t{enums}");
            }

            if (namespaces > 0)
            {
                this.console.WriteLine($"Namespaces:\t{namespaces}");
            }

            if (interfaces > 0)
            {
                this.console.WriteLine($"Interfaces:\t{interfaces}");
            }

            if (fields > 0)
            {
                this.console.WriteLine($"Fields:\t{fields}");
            }

            if (props > 0)
            {
                this.console.WriteLine($"Properties:\t{props}");
            }

            if (methods > 0)
            {
                this.console.WriteLine($"Methods:\t{methods}");
            }

            if (args > 0)
            {
                this.console.WriteLine($"Arguments:\t{args}");
            }

            if (localVariables > 0)
            {
                this.console.WriteLine($"Local Variables:\t{localVariables}");
            }
        }
    }
}
