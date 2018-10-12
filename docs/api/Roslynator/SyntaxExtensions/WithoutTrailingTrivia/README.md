<a name="_top"></a>

# SyntaxExtensions\.WithoutTrailingTrivia Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithoutTrailingTrivia(SyntaxNodeOrToken)](#Roslynator_SyntaxExtensions_WithoutTrailingTrivia_Microsoft_CodeAnalysis_SyntaxNodeOrToken_) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the trailing trivia removed\. |
| [WithoutTrailingTrivia(SyntaxToken)](#Roslynator_SyntaxExtensions_WithoutTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new token from this token with the trailing trivia removed\. |

## WithoutTrailingTrivia\(SyntaxNodeOrToken\) <a name="Roslynator_SyntaxExtensions_WithoutTrailingTrivia_Microsoft_CodeAnalysis_SyntaxNodeOrToken_"></a>

### Summary

Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the trailing trivia removed\.

```csharp
public static SyntaxNodeOrToken WithoutTrailingTrivia(this SyntaxNodeOrToken nodeOrToken)
```

### Parameters

**nodeOrToken**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken)

## WithoutTrailingTrivia\(SyntaxToken\) <a name="Roslynator_SyntaxExtensions_WithoutTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_"></a>

### Summary

Creates a new token from this token with the trailing trivia removed\.

```csharp
public static SyntaxToken WithoutTrailingTrivia(this SyntaxToken token)
```

### Parameters

**token**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

