<a name="_top"></a>

# SyntaxTreeExtensions Class

[Home](../../README.md#_top) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

A set of extension methods for [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static class SyntaxTreeExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [GetEndLine(SyntaxTree, TextSpan, CancellationToken)](GetEndLine/README.md#_top) | Returns zero\-based index of the end line of the specified span\. |
| [GetStartLine(SyntaxTree, TextSpan, CancellationToken)](GetStartLine/README.md#_top) | Returns zero\-based index of the start line of the specified span\. |
| [IsMultiLineSpan(SyntaxTree, TextSpan, CancellationToken)](IsMultiLineSpan/README.md#_top) | Returns true if the specified [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan) spans over multiple lines\. |
| [IsSingleLineSpan(SyntaxTree, TextSpan, CancellationToken)](IsSingleLineSpan/README.md#_top) | Returns true if the specified [TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan) does not span over multiple lines\. |

