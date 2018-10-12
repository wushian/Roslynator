<a name="_top"></a>

# SyntaxExtensions\.WithoutLeadingTrivia Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithoutLeadingTrivia(SyntaxNodeOrToken)](#Roslynator_SyntaxExtensions_WithoutLeadingTrivia_Microsoft_CodeAnalysis_SyntaxNodeOrToken_) | Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the leading trivia removed\. |
| [WithoutLeadingTrivia(SyntaxToken)](#Roslynator_SyntaxExtensions_WithoutLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new token from this token with the leading trivia removed\. |

## WithoutLeadingTrivia\(SyntaxNodeOrToken\) <a name="Roslynator_SyntaxExtensions_WithoutLeadingTrivia_Microsoft_CodeAnalysis_SyntaxNodeOrToken_"></a>

### Summary

Creates a new [SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken) with the leading trivia removed\.

```csharp
public static SyntaxNodeOrToken WithoutLeadingTrivia(this SyntaxNodeOrToken nodeOrToken)
```

### Parameters

**nodeOrToken**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxNodeOrToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnodeortoken)

## WithoutLeadingTrivia\(SyntaxToken\) <a name="Roslynator_SyntaxExtensions_WithoutLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_"></a>

### Summary

Creates a new token from this token with the leading trivia removed\.

```csharp
public static SyntaxToken WithoutLeadingTrivia(this SyntaxToken token)
```

### Parameters

**token**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

