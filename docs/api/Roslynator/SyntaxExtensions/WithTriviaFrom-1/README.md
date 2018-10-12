<a name="_top"></a>

# SyntaxExtensions\.WithTriviaFrom Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithTriviaFrom(SyntaxToken, SyntaxNode)](../WithTriviaFrom/README.md#Roslynator_SyntaxExtensions_WithTriviaFrom_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxNode_) | Creates a new token from this token with both the leading and trailing trivia of the specified node\. |
| [WithTriviaFrom\<TNode>(SeparatedSyntaxList\<TNode>, SyntaxNode)](#Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_) | Creates a new separated list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom\<TNode>(SyntaxList\<TNode>, SyntaxNode)](#Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_) | Creates a new list with both leading and trailing trivia of the specified node\. If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\. |
| [WithTriviaFrom\<TNode>(TNode, SyntaxToken)](#Roslynator_SyntaxExtensions_WithTriviaFrom__1___0_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new node from this node with both the leading and trailing trivia of the specified token\. |

## WithTriviaFrom\(SyntaxToken, SyntaxNode\) <a name="Roslynator_SyntaxExtensions_WithTriviaFrom_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxNode_"></a>

### Summary

Creates a new token from this token with both the leading and trailing trivia of the specified node\.

```csharp
public static SyntaxToken WithTriviaFrom(this SyntaxToken token, SyntaxNode node)
```

### Parameters

**token**

**node**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## WithTriviaFrom\<TNode>\(SeparatedSyntaxList\<TNode>, SyntaxNode\) <a name="Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_"></a>

### Summary

Creates a new separated list with both leading and trailing trivia of the specified node\.
If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\.

```csharp
public static SeparatedSyntaxList<TNode> WithTriviaFrom<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**node**

### Returns

Microsoft\.CodeAnalysis\.[SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)

## WithTriviaFrom\<TNode>\(SyntaxList\<TNode>, SyntaxNode\) <a name="Roslynator_SyntaxExtensions_WithTriviaFrom__1_Microsoft_CodeAnalysis_SyntaxList___0__Microsoft_CodeAnalysis_SyntaxNode_"></a>

### Summary

Creates a new list with both leading and trailing trivia of the specified node\.
If the list contains more than one item, first item is updated with leading trivia and last item is updated with trailing trivia\.

```csharp
public static SyntaxList<TNode> WithTriviaFrom<TNode>(this SyntaxList<TNode> list, SyntaxNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**node**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)

## WithTriviaFrom\<TNode>\(TNode, SyntaxToken\) <a name="Roslynator_SyntaxExtensions_WithTriviaFrom__1___0_Microsoft_CodeAnalysis_SyntaxToken_"></a>

### Summary

Creates a new node from this node with both the leading and trailing trivia of the specified token\.

```csharp
public static TNode WithTriviaFrom<TNode>(this TNode node, SyntaxToken token) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node**

**token**

### Returns

TNode

