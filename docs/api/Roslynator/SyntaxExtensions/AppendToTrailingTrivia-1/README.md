<a name="_top"></a>

# SyntaxExtensions\.AppendToTrailingTrivia Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [AppendToTrailingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia>)](../AppendToTrailingTrivia/README.md#Roslynator_SyntaxExtensions_AppendToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia(SyntaxToken, SyntaxTrivia)](../AppendToTrailingTrivia/README.md#Roslynator_SyntaxExtensions_AppendToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia\<TNode>(TNode, IEnumerable\<SyntaxTrivia>)](#Roslynator_SyntaxExtensions_AppendToTrailingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |
| [AppendToTrailingTrivia\<TNode>(TNode, SyntaxTrivia)](#Roslynator_SyntaxExtensions_AppendToTrailingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\. |

## AppendToTrailingTrivia\(SyntaxToken, IEnumerable\<SyntaxTrivia>\) <a name="Roslynator_SyntaxExtensions_AppendToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

### Summary

Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\.

```csharp
public static SyntaxToken AppendToTrailingTrivia(this SyntaxToken token, IEnumerable<SyntaxTrivia> trivia)
```

### Parameters

**token**

**trivia**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## AppendToTrailingTrivia\(SyntaxToken, SyntaxTrivia\) <a name="Roslynator_SyntaxExtensions_AppendToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

### Summary

Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\.

```csharp
public static SyntaxToken AppendToTrailingTrivia(this SyntaxToken token, SyntaxTrivia trivia)
```

### Parameters

**token**

**trivia**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## AppendToTrailingTrivia\<TNode>\(TNode, IEnumerable\<SyntaxTrivia>\) <a name="Roslynator_SyntaxExtensions_AppendToTrailingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

### Summary

Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\.

```csharp
public static TNode AppendToTrailingTrivia<TNode>(this TNode node, IEnumerable<SyntaxTrivia> trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node**

**trivia**

### Returns

TNode

## AppendToTrailingTrivia\<TNode>\(TNode, SyntaxTrivia\) <a name="Roslynator_SyntaxExtensions_AppendToTrailingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

### Summary

Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is added at the end of the trailing trivia\.

```csharp
public static TNode AppendToTrailingTrivia<TNode>(this TNode node, SyntaxTrivia trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node**

**trivia**

### Returns

TNode

