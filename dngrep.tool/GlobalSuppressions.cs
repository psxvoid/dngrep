// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "It's fine until this method is disposed manually.", Scope = "type", Target = "~T:dngrep.tool.Abstractions.MSBuildWorkspaceDecorator")]
[assembly: SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "It's fine until this method is disposed manually.", Scope = "member", Target = "~M:dngrep.tool.Abstractions.MSBuildWorkspaceDecorator.Dispose")]
[assembly: SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "It's fine until this method is disposed manually.", Scope = "member", Target = "~M:dngrep.tool.Abstractions.MSBuildWorkspaceDecorator.Dispose")]
