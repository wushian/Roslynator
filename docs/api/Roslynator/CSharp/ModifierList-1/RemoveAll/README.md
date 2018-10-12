<a name="_top"></a>

# ModifierList\.RemoveAll Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[ModifierList\<TNode>](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RemoveAll(TNode)](#Roslynator_CSharp_ModifierList_1_RemoveAll__0_) | Creates a new node with all modifiers removed\. |
| [RemoveAll(TNode, Func\<SyntaxToken, Boolean>)](#Roslynator_CSharp_ModifierList_1_RemoveAll__0_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__) | Creates a new node with modifiers that matches the predicate removed\. |

## RemoveAll\(TNode\) <a name="Roslynator_CSharp_ModifierList_1_RemoveAll__0_"></a>

### Summary

Creates a new node with all modifiers removed\.

```csharp
public TNode RemoveAll(TNode node)
```

### Parameters

**node**

### Returns

TNode

## RemoveAll\(TNode, Func\<SyntaxToken, Boolean>\) <a name="Roslynator_CSharp_ModifierList_1_RemoveAll__0_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__"></a>

### Summary

Creates a new node with modifiers that matches the predicate removed\.

```csharp
public TNode RemoveAll(TNode node, Func<SyntaxToken, bool> predicate)
```

### Parameters

**node**

**predicate**

### Returns

TNode

