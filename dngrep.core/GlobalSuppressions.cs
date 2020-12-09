// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "For now it is a single-language tool.", Scope = "type", Target = "~T:dngrep.core.Queries.SyntaxNodeMatchers.Boolean.Not")]
[assembly: SuppressMessage("Naming", "CA1724:Type names should not match namespaces", Justification = "Acceptable by the project's style guide.", Scope = "type", Target = "~T:dngrep.core.Extensions.SourceTextExtensions.SourceTextExtensions")]
[assembly: SuppressMessage("Naming", "CA1724:Type names should not match namespaces", Justification = "Acceptable by the project's style guide.", Scope = "type", Target = "~T:dngrep.core.Extensions.SyntaxTreeExtensions.SyntaxTreeExtensions")]
[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Acceptable by the project's style guide.", Scope = "type", Target = "~T:dngrep.core.Queries.SyntaxNodeMatchers.Boolean.And")]
[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Acceptable by the project's style guide.", Scope = "namespace", Target = "~N:dngrep.core.Queries.SyntaxNodeMatchers.Boolean")]
[assembly: SuppressMessage("Style", "IDE0056:Use index operator", Justification = "The indexer conflicts with the target framework.", Scope = "member", Target = "~M:dngrep.core.Queries.SyntaxWalkers.SyntaxTreeQueryWalkerBase`1.PeekResult~`0")]
