<a name="_top"></a>

# SyntaxExtensions\.ReplaceAt Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReplaceAt(SyntaxTokenList, Int32, SyntaxToken)](../ReplaceAt/README.md#Roslynator_SyntaxExtensions_ReplaceAt_Microsoft_CodeAnalysis_SyntaxTokenList_System_Int32_Microsoft_CodeAnalysis_SyntaxToken_) | Creates a new [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) with a token at the specified index replaced with a new token\. |
| [ReplaceAt(SyntaxTriviaList, Int32, SyntaxTrivia)](../ReplaceAt/README.md#Roslynator_SyntaxExtensions_ReplaceAt_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Int32_Microsoft_CodeAnalysis_SyntaxTrivia_) | Creates a new [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) with a trivia at the specified index replaced with new trivia\. |
| [ReplaceAt\<TNode>(SeparatedSyntaxList\<TNode>, Int32, TNode)](#Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32___0_) | Creates a new list with a node at the specified index replaced with a new node\. |
| [ReplaceAt\<TNode>(SyntaxList\<TNode>, Int32, TNode)](#Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32___0_) | Creates a new list with the node at the specified index replaced with a new node\. |

## ReplaceAt\(SyntaxTokenList, Int32, SyntaxToken\) <a name="Roslynator_SyntaxExtensions_ReplaceAt_Microsoft_CodeAnalysis_SyntaxTokenList_System_Int32_Microsoft_CodeAnalysis_SyntaxToken_"></a>

### Summary

Creates a new [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) with a token at the specified index replaced with a new token\.

```csharp
public static SyntaxTokenList ReplaceAt(this SyntaxTokenList tokenList, int index, SyntaxToken newToken)
```

### Parameters

**tokenList**

**index**

**newToken**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

## ReplaceAt\(SyntaxTriviaList, Int32, SyntaxTrivia\) <a name="Roslynator_SyntaxExtensions_ReplaceAt_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Int32_Microsoft_CodeAnalysis_SyntaxTrivia_"></a>

### Summary

Creates a new [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) with a trivia at the specified index replaced with new trivia\.

```csharp
public static SyntaxTriviaList ReplaceAt(this SyntaxTriviaList triviaList, int index, SyntaxTrivia newTrivia)
```

### Parameters

**triviaList**

**index**

**newTrivia**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

## ReplaceAt\<TNode>\(SeparatedSyntaxList\<TNode>, Int32, TNode\) <a name="Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32___0_"></a>

### Summary

Creates a new list with a node at the specified index replaced with a new node\.

```csharp
public static SeparatedSyntaxList<TNode> ReplaceAt<TNode>(this SeparatedSyntaxList<TNode> list, int index, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**index**

**newNode**

### Returns

Microsoft\.CodeAnalysis\.[SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)

## ReplaceAt\<TNode>\(SyntaxList\<TNode>, Int32, TNode\) <a name="Roslynator_SyntaxExtensions_ReplaceAt__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32___0_"></a>

### Summary

Creates a new list with the node at the specified index replaced with a new node\.

```csharp
public static SyntaxList<TNode> ReplaceAt<TNode>(this SyntaxList<TNode> list, int index, TNode newNode) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**index**

**newNode**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)

