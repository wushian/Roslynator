<a name="_top"></a>

# SyntaxExtensions\.IsLast Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsLast\<TNode>(SeparatedSyntaxList\<TNode>, TNode)](#Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0____0_) | Returns true if the specified node is a last node in the list\. |
| [IsLast\<TNode>(SyntaxList\<TNode>, TNode)](#Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SyntaxList___0____0_) | Returns true if the specified node is a last node in the list\. |

## IsLast\<TNode>\(SeparatedSyntaxList\<TNode>, TNode\) <a name="Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0____0_"></a>

### Summary

Returns true if the specified node is a last node in the list\.

```csharp
public static bool IsLast<TNode>(this SeparatedSyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**node**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsLast\<TNode>\(SyntaxList\<TNode>, TNode\) <a name="Roslynator_SyntaxExtensions_IsLast__1_Microsoft_CodeAnalysis_SyntaxList___0____0_"></a>

### Summary

Returns true if the specified node is a last node in the list\.

```csharp
public static bool IsLast<TNode>(this SyntaxList<TNode> list, TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**node**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

