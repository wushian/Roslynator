<a name="_top"></a>

# SyntaxExtensions\.Contains Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Contains(SyntaxTokenList, SyntaxToken)](../Contains/README.md#Roslynator_SyntaxExtensions_Contains_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_) | Returns true if the specified token is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\. |
| [Contains\<TNode>(SeparatedSyntaxList\<TNode>, TNode)](#Roslynator_SyntaxExtensions_Contains__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0____0_) | Returns true if the specified node is in the [SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\. |
| [Contains\<TNode>(SyntaxList\<TNode>, TNode)](#Roslynator_SyntaxExtensions_Contains__1_Microsoft_CodeAnalysis_SyntaxList___0____0_) | Returns true if the specified node is in the [SyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\. |

## Contains\(SyntaxTokenList, SyntaxToken\) <a name="Roslynator_SyntaxExtensions_Contains_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_"></a>

### Summary

Returns true if the specified token is in the [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)\.

```csharp
public static bool Contains(this SyntaxTokenList tokens, SyntaxToken token)
```

### Parameters

**tokens**

**token**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## Contains\<TNode>\(SeparatedSyntaxList\<TNode>, TNode\) <a name="Roslynator_SyntaxExtensions_Contains__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0____0_"></a>

### Summary

Returns true if the specified node is in the [SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)\.

```csharp
public static bool Contains<TNode>(this SeparatedSyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**node**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## Contains\<TNode>\(SyntaxList\<TNode>, TNode\) <a name="Roslynator_SyntaxExtensions_Contains__1_Microsoft_CodeAnalysis_SyntaxList___0____0_"></a>

### Summary

Returns true if the specified node is in the [SyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)\.

```csharp
public static bool Contains<TNode>(this SyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**node**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

