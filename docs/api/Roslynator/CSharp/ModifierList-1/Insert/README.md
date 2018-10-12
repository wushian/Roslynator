<a name="_top"></a>

# ModifierList\.Insert Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[ModifierList\<TNode>](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Insert(TNode, SyntaxKind, IComparer\<SyntaxKind>)](#Roslynator_CSharp_ModifierList_1_Insert__0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Creates a new node with a modifier of the specified kind inserted\. |
| [Insert(TNode, SyntaxToken, IComparer\<SyntaxToken>)](#Roslynator_CSharp_ModifierList_1_Insert__0_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Creates a new node with the specified modifier inserted\. |

## Insert\(TNode, SyntaxKind, IComparer\<SyntaxKind>\) <a name="Roslynator_CSharp_ModifierList_1_Insert__0_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__"></a>

### Summary

Creates a new node with a modifier of the specified kind inserted\.

```csharp
public TNode Insert(TNode node, SyntaxKind kind, IComparer<SyntaxKind> comparer = null)
```

### Parameters

**node**

**kind**

**comparer**

### Returns

TNode

## Insert\(TNode, SyntaxToken, IComparer\<SyntaxToken>\) <a name="Roslynator_CSharp_ModifierList_1_Insert__0_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__"></a>

### Summary

Creates a new node with the specified modifier inserted\.

```csharp
public TNode Insert(TNode node, SyntaxToken modifier, IComparer<SyntaxToken> comparer = null)
```

### Parameters

**node**

**modifier**

**comparer**

### Returns

TNode

