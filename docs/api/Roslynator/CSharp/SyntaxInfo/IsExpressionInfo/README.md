# SyntaxInfo\.IsExpressionInfo Method

[Home](../../../../README.md)

**Containing Type**: Roslynator\.CSharp\.[SyntaxInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsExpressionInfo(BinaryExpressionSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_IsExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_BinaryExpressionSyntax_System_Boolean_System_Boolean_) | Creates a new [IsExpressionInfo](../../Syntax/IsExpressionInfo/README.md) from the specified binary expression\. |
| [IsExpressionInfo(SyntaxNode, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_IsExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_) | Creates a new [IsExpressionInfo](../../Syntax/IsExpressionInfo/README.md) from the specified node\. |

## IsExpressionInfo\(BinaryExpressionSyntax, Boolean, Boolean\) <a name="Roslynator_CSharp_SyntaxInfo_IsExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_BinaryExpressionSyntax_System_Boolean_System_Boolean_"></a>

### Summary

Creates a new [IsExpressionInfo](../../Syntax/IsExpressionInfo/README.md) from the specified binary expression\.

```csharp
public static IsExpressionInfo IsExpressionInfo(BinaryExpressionSyntax binaryExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**binaryExpression**

**walkDownParentheses**

**allowMissing**

### Returns

Roslynator\.CSharp\.Syntax\.[IsExpressionInfo](../../Syntax/IsExpressionInfo/README.md)

## IsExpressionInfo\(SyntaxNode, Boolean, Boolean\) <a name="Roslynator_CSharp_SyntaxInfo_IsExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_"></a>

### Summary

Creates a new [IsExpressionInfo](../../Syntax/IsExpressionInfo/README.md) from the specified node\.

```csharp
public static IsExpressionInfo IsExpressionInfo(SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node**

**walkDownParentheses**

**allowMissing**

### Returns

Roslynator\.CSharp\.Syntax\.[IsExpressionInfo](../../Syntax/IsExpressionInfo/README.md)

