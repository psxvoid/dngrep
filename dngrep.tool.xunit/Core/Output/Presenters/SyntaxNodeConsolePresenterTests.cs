﻿using System;
using System.Collections.Generic;
using AutoFixture;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.tool.Abstractions.System;
using dngrep.tool.Core.Options;
using dngrep.tool.Core.Output.Presenters;
using dngrep.tool.xunit.TestHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Xunit;

namespace dngrep.tool.xunit.Core.Output.Presenters
{
    public static class SyntaxNodeConsolePresenterTests
    {
        public class MethodInClassInNamespaceWithFilePath
        {
            private const string SourceCode = @"
                namespace SolarSystem
                {
                    public class Earth
                    {
                        public string Spin()
                        {
                            string action = ""Spinning!"";
                            return string;
                        }
                    }
                }
            ";

            private readonly IFixture fixture;
            private readonly SyntaxTree syntaxTree;
            private readonly IEnumerable<SyntaxNode> nodes;

            private readonly Mock<IStringConsole> consoleMock;

            private readonly ConsoleSyntaxNodePresenter sut;

            public MethodInClassInNamespaceWithFilePath()
            {
                this.syntaxTree = CSharpSyntaxTree.ParseText(SourceCode, null, "x:/code.cs");
                this.nodes = this.syntaxTree
                    .GetRoot()
                    .ChildNodes()
                    .GetNodesOfTypeRecursively<MethodDeclarationSyntax>();

                this.fixture = AutoFixtureFactory.Default();
                this.consoleMock = this.fixture.Freeze<Mock<IStringConsole>>();

                this.sut = this.fixture.Create<ConsoleSyntaxNodePresenter>();
            }

            [Fact]
            public void ProduceOutput_FullName_FullName()
            {
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .OmitAutoProperties()
                    .With(x => x.ShowFullName, true)
                    .Create();

                this.sut.ProduceOutput(this.nodes, options);

                this.consoleMock.VerifyWriteLine("SolarSystem.Earth.Spin", Times.Once());
                this.consoleMock.VerifyWriteLineAny(Times.Once());
            }

            [Fact]
            public void ProduceOutput_Path_NameAndPath()
            {
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .OmitAutoProperties()
                    .With(x => x.ShowFilePath, true)
                    .Create();

                this.sut.ProduceOutput(this.nodes, options);

                this.consoleMock.VerifyWriteLine("Spin", Times.Once());
                this.consoleMock.VerifyWrite($"\tat x:/code.cs{Environment.NewLine}", Times.Once());
                this.consoleMock.VerifyWriteLineAny(Times.Once());
                this.consoleMock.VerifyWriteAny(Times.Once());
            }

            [Fact]
            public void ProduceOutput_Position_NamePosition()
            {
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .OmitAutoProperties()
                    .With(x => x.ShowPosition, true)
                    .Create();

                this.sut.ProduceOutput(this.nodes, options);

                this.consoleMock.VerifyWriteLine("Spin", Times.Once());
                this.consoleMock.VerifyWriteLine($"\tat line {5}, char {24}", Times.Once());
                this.consoleMock.VerifyWriteLineAny(Times.Exactly(2));
            }

            [Fact]
            public void ProduceOutput_FullNameAndPath_FullNameAndPath()
            {
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .OmitAutoProperties()
                    .With(x => x.ShowFullName, true)
                    .With(x => x.ShowFilePath, true)
                    .Create();

                this.sut.ProduceOutput(this.nodes, options);

                this.consoleMock.VerifyWriteLine("SolarSystem.Earth.Spin", Times.Once());
                this.consoleMock.VerifyWrite($"\tat x:/code.cs{Environment.NewLine}", Times.Once());
                this.consoleMock.VerifyWriteLineAny(Times.Once());
                this.consoleMock.VerifyWriteAny(Times.Once());
            }

            [Fact]
            public void ProduceOutput_FullNameAndPathAndPosition_FullNameAndPath()
            {
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .OmitAutoProperties()
                    .With(x => x.ShowFullName, true)
                    .With(x => x.ShowFilePath, true)
                    .With(x => x.ShowPosition, true)
                    .Create();

                this.sut.ProduceOutput(this.nodes, options);

                this.consoleMock.VerifyWriteLine("SolarSystem.Earth.Spin", Times.Once());
                this.consoleMock.VerifyWrite("\tat x:/code.cs", Times.Once());
                this.consoleMock.VerifyWriteLine(":line 5, char 24", Times.Once());
                this.consoleMock.VerifyWriteLineAny(Times.Exactly(2));
                this.consoleMock.VerifyWriteAny(Times.Once());
            }

            [Fact]
            public void ProduceOutput_FullNameAndHideNamespaces_NoNamespaces()
            {
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .OmitAutoProperties()
                    .With(x => x.ShowFullName, true)
                    .With(x => x.HideNamespaces, true)
                    .Create();

                this.sut.ProduceOutput(this.nodes, options);

                this.consoleMock.VerifyWriteLine("Earth.Spin", Times.Once());
                this.consoleMock.VerifyWriteLineAny(Times.Exactly(1));
            }
        }

        /// <summary>
        /// The tests verifies the case where a node has no
        /// known way to get it's name using <see cref="SyntaxNodeExtensions.TryGetFullName"/>
        /// or <see cref="SyntaxNodeExtensions.TryGetIdentifierName"/> method.
        /// </summary>
        public class NodeWithoutKnownName
        {
            private const string SourceCode = @"
                using System;
            ";

            private readonly IFixture fixture;
            private readonly SyntaxTree syntaxTree;
            private readonly IEnumerable<SyntaxNode> nodes;

            private readonly Mock<IStringConsole> consoleMock;

            private readonly ConsoleSyntaxNodePresenter sut;

            public NodeWithoutKnownName()
            {
                this.syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);
                this.nodes = this.syntaxTree
                    .GetRoot()
                    .ChildNodes()
                    .GetNodesOfTypeRecursively<MethodDeclarationSyntax>();

                this.fixture = AutoFixtureFactory.Default();
                this.consoleMock = this.fixture.Freeze<Mock<IStringConsole>>();

                this.sut = this.fixture.Create<ConsoleSyntaxNodePresenter>();
            }

            [Fact]
            public void ProduceOutput_UsingDirectiveAndFullName_NoOutput()
            {
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .OmitAutoProperties()
                    .With(x => x.ShowFullName, true)
                    .Create();

                this.sut.ProduceOutput(this.nodes, options);

                this.consoleMock.VerifyWriteLineAny(Times.Never());
            }

            [Fact]
            public void ProduceOutput_UsingDirectiveAndNoFullName_NoOutput()
            {
                GrepOptions options = this.fixture.Build<GrepOptions>()
                    .OmitAutoProperties()
                    .Create();

                this.sut.ProduceOutput(this.nodes, options);

                this.consoleMock.VerifyWriteLineAny(Times.Never());
            }
        }
    }
}
