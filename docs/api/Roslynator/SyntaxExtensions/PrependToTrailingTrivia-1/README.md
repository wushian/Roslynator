<a name="_top"></a>

# SyntaxExtensions\.PrependToTrailingTrivia Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [PrependToTrailingTrivia(SyntaxToken, IEnumerable\<SyntaxTrivia>)](../PrependToTrailingTrivia/README.md#Roslynator_SyntaxExtensions_PrependToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the begining of the trailing trivia\. |
| [PrependToTrailingTrivia(SyntaxToken, SyntaxTrivia)](../PrependToTrailingTrivia/README.md#Roslynator_SyntaxExtensions_PrependToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the begining of the trailing trivia\. |
| [PrependToTrailingTrivia\<TNode>(TNode, IEnumerable\<SyntaxTrivia>)](#Roslynator_SyntaxExtensions_PrependToTrailingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the begining of the trailing trivia\. |
| [PrependToTrailingTrivia\<TNode>(TNode, SyntaxTrivia)](#Roslynator_SyntaxExtensions_PrependToTrailingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the begining of the trailing trivia\. |

## PrependToTrailingTrivia\(SyntaxToken, IEnumerable\<SyntaxTrivia>\) <a name="Roslynator_SyntaxExtensions_PrependToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

### Summary

Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the begining of the trailing trivia\.

```csharp
public static SyntaxToken PrependToTrailingTrivia(this SyntaxToken token, IEnumerable<SyntaxTrivia> trivia)
```

### Parameters

**token**

**trivia**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## PrependToTrailingTrivia\(SyntaxToken, SyntaxTrivia\) <a name="Roslynator_SyntaxExtensions_PrependToTrailingTrivia_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

### Summary

Creates a new token from this token with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the begining of the trailing trivia\.

```csharp
public static SyntaxToken PrependToTrailingTrivia(this SyntaxToken token, SyntaxTrivia trivia)
```

### Parameters

**token**

**trivia**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## PrependToTrailingTrivia\<TNode>\(TNode, IEnumerable\<SyntaxTrivia>\) <a name="Roslynator_SyntaxExtensions_PrependToTrailingTrivia__1___0_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxTrivia__"></a>

### Summary

Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the begining of the trailing trivia\.

```csharp
public static TNode PrependToTrailingTrivia<TNode>(this TNode node, IEnumerable<SyntaxTrivia> trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node**

**trivia**

### Returns

TNode

## PrependToTrailingTrivia\<TNode>\(TNode, SyntaxTrivia\) <a name="Roslynator_SyntaxExtensions_PrependToTrailingTrivia__1___0_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

### Summary

Creates a new node from this node with the trailing trivia replaced with a new trivia where the specified trivia is inserted at the begining of the trailing trivia\.

```csharp
public static TNode PrependToTrailingTrivia<TNode>(this TNode node, SyntaxTrivia trivia) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node**

**trivia**

### Returns

TNode

