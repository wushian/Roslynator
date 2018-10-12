<a name="_top"></a>

# SyntaxExtensions\.ToSeparatedSyntaxList Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ToSeparatedSyntaxList\<TNode>(IEnumerable\<SyntaxNodeOrToken>)](#Roslynator_CSharp_SyntaxExtensions_ToSeparatedSyntaxList__1_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxNodeOrToken__) | Creates a separated list of syntax nodes from a sequence of nodes and tokens\. |
| [ToSeparatedSyntaxList\<TNode>(IEnumerable\<TNode>)](#Roslynator_CSharp_SyntaxExtensions_ToSeparatedSyntaxList__1_System_Collections_Generic_IEnumerable___0__) | Creates a separated list of syntax nodes from a sequence of nodes\. |

## ToSeparatedSyntaxList\<TNode>\(IEnumerable\<SyntaxNodeOrToken>\) <a name="Roslynator_CSharp_SyntaxExtensions_ToSeparatedSyntaxList__1_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_SyntaxNodeOrToken__"></a>

### Summary

Creates a separated list of syntax nodes from a sequence of nodes and tokens\.

```csharp
public static SeparatedSyntaxList<TNode> ToSeparatedSyntaxList<TNode>(this IEnumerable<SyntaxNodeOrToken> nodesAndTokens) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**nodesAndTokens**

### Returns

Microsoft\.CodeAnalysis\.[SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)

## ToSeparatedSyntaxList\<TNode>\(IEnumerable\<TNode>\) <a name="Roslynator_CSharp_SyntaxExtensions_ToSeparatedSyntaxList__1_System_Collections_Generic_IEnumerable___0__"></a>

### Summary

Creates a separated list of syntax nodes from a sequence of nodes\.

```csharp
public static SeparatedSyntaxList<TNode> ToSeparatedSyntaxList<TNode>(this IEnumerable<TNode> nodes) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**nodes**

### Returns

Microsoft\.CodeAnalysis\.[SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)

