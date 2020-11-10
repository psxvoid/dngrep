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

        private int classes;
        private int structs;
        private int enums;
        private int namespaces;
        private int interfaces;
        private int fields;
        private int props;
        private int methods;
        private int args;
        private int localVariables;

        public SyntaxNodeStatisticsConsolePresenter(IConsole console)
        {
            this.console = console;
        }

        public void ProduceOutput(IEnumerable<SyntaxNode> nodes, GrepOptions options)
        {
            _ = nodes ?? throw new ArgumentNullException(nameof(nodes));


            foreach (var node in nodes)
            {
                Type nodeType = node.GetType();

                if (nodeType == typeof(ClassDeclarationSyntax))
                {
                    this.classes++;
                }
                else if (nodeType == typeof(StructDeclarationSyntax))
                {
                    this.structs++;
                }
                else if (nodeType == typeof(EnumDeclarationSyntax))
                {
                    this.enums++;
                }
                else if (nodeType == typeof(NamespaceDeclarationSyntax))
                {
                    this.namespaces++;
                }
                else if (nodeType == typeof(InterfaceDeclarationSyntax))
                {
                    this.interfaces++;
                }
                else if (nodeType == typeof(FieldDeclarationSyntax))
                {
                    this.fields++;
                }
                else if (nodeType == typeof(PropertyDeclarationSyntax))
                {
                    this.props++;
                }
                else if (nodeType == typeof(MethodDeclarationSyntax))
                {
                    this.methods++;
                }
                else if (nodeType == typeof(ArgumentSyntax))
                {
                    this.args++;
                }
                else if (nodeType == typeof(LocalDeclarationStatementSyntax))
                {
                    this.localVariables++;
                }
            }

        }

        public void Flush()
        {
            if (this.classes > 0)
            {
                this.console.WriteLine($"Classes:\t{this.classes}");
            }

            if (this.structs > 0)
            {
                this.console.WriteLine($"Structs:\t{this.structs}");
            }

            if (this.enums > 0)
            {
                this.console.WriteLine($"Enums:\t\t{this.enums}");
            }

            if (this.namespaces > 0)
            {
                this.console.WriteLine($"Namespaces:\t{this.namespaces}");
            }

            if (this.interfaces > 0)
            {
                this.console.WriteLine($"Interfaces:\t{this.interfaces}");
            }

            if (this.fields > 0)
            {
                this.console.WriteLine($"Fields:\t\t{this.fields}");
            }

            if (this.props > 0)
            {
                this.console.WriteLine($"Properties:\t{this.props}");
            }

            if (this.methods > 0)
            {
                this.console.WriteLine($"Methods:\t{this.methods}");
            }

            if (this.args > 0)
            {
                this.console.WriteLine($"Arguments:\t{this.args}");
            }

            if (this.localVariables > 0)
            {
                this.console.WriteLine($"Local Vars:\t{this.localVariables}");
            }

            this.Reset();
        }

        private void Reset()
        {
            this.classes = 0;
            this.structs = 0;
            this.enums = 0;
            this.namespaces = 0;
            this.interfaces = 0;
            this.fields = 0;
            this.props = 0;
            this.methods = 0;
            this.args = 0;
            this.localVariables = 0;
        }
    }
}
