<a name="_top"></a>

# SyntaxExtensions\.FirstDescendant Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [FirstDescendant\<TNode>(SyntaxNode, Func\<SyntaxNode, Boolean>, Boolean)](#Roslynator_SyntaxExtensions_FirstDescendant__1_Microsoft_CodeAnalysis_SyntaxNode_System_Func_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean__System_Boolean_) | Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\. |
| [FirstDescendant\<TNode>(SyntaxNode, TextSpan, Func\<SyntaxNode, Boolean>, Boolean)](#Roslynator_SyntaxExtensions_FirstDescendant__1_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_Text_TextSpan_System_Func_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean__System_Boolean_) | Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\. |

## FirstDescendant\<TNode>\(SyntaxNode, Func\<SyntaxNode, Boolean>, Boolean\) <a name="Roslynator_SyntaxExtensions_FirstDescendant__1_Microsoft_CodeAnalysis_SyntaxNode_System_Func_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean__System_Boolean_"></a>

### Summary

Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\.

```csharp
public static TNode FirstDescendant<TNode>(this SyntaxNode node, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node**

**descendIntoChildren**

**descendIntoTrivia**

### Returns

TNode

## FirstDescendant\<TNode>\(SyntaxNode, TextSpan, Func\<SyntaxNode, Boolean>, Boolean\) <a name="Roslynator_SyntaxExtensions_FirstDescendant__1_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_Text_TextSpan_System_Func_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean__System_Boolean_"></a>

### Summary

Searches a list of descendant nodes in prefix document order and returns first descendant of type **TNode**\.

```csharp
public static TNode FirstDescendant<TNode>(this SyntaxNode node, TextSpan span, Func<SyntaxNode, bool> descendIntoChildren = null, bool descendIntoTrivia = false) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node**

**span**

**descendIntoChildren**

**descendIntoTrivia**

### Returns

TNode

