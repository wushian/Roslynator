<a name="_top"></a>

# SyntaxExtensions\.AppendToLeadingTrivia Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [AppendToLeadingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia>)](../AppendToLeadingTrivia/README.md#Roslynator_SyntaxExtensions_AppendToLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia(SyntaxToken, SyntaxTrivia)](../AppendToLeadingTrivia/README.md#Roslynator_SyntaxExtensions_AppendToLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia\<TNode>(TNode, IEnumerable\<SyntaxTrivia>)](#Roslynator_SyntaxExtensions_AppendToLeadingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |
| [AppendToLeadingTrivia\<TNode>(TNode, SyntaxTrivia)](#Roslynator_SyntaxExtensions_AppendToLeadingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\. |

## AppendToLeadingTrivia\(SyntaxToken, IEnumerable\<SyntaxTrivia>\) <a name="Roslynator_SyntaxExtensions_AppendToLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

### Summary

Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\.

```csharp
public static SyntaxToken AppendToLeadingTrivia(this SyntaxToken token, IEnumerable<SyntaxTrivia> trivia)
```

### Parameters

**token**

**trivia**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## AppendToLeadingTrivia\(SyntaxToken, SyntaxTrivia\) <a name="Roslynator_SyntaxExtensions_AppendToLeadingTrivia_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

### Summary

Creates a new token from this token with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\.

```csharp
public static SyntaxToken AppendToLeadingTrivia(this SyntaxToken token, SyntaxTrivia trivia)
```

### Parameters

**token**

**trivia**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## AppendToLeadingTrivia\<TNode>\(TNode, IEnumerable\<SyntaxTrivia>\) <a name="Roslynator_SyntaxExtensions_AppendToLeadingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

### Summary

Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\.

```csharp
public static TNode AppendToLeadingTrivia<TNode>(this TNode node, IEnumerable<SyntaxTrivia> trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node**

**trivia**

### Returns

TNode

## AppendToLeadingTrivia\<TNode>\(TNode, SyntaxTrivia\) <a name="Roslynator_SyntaxExtensions_AppendToLeadingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

### Summary

Creates a new node from this node with the leading trivia replaced with a new trivia where the specified trivia is added at the end of the leading trivia\.

```csharp
public static TNode AppendToLeadingTrivia<TNode>(this TNode node, SyntaxTrivia trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node**

**trivia**

### Returns

TNode

