<a name="_top"></a>

# SyntaxInfo\.AssignmentExpressionInfo Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxInfo](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [AssignmentExpressionInfo(AssignmentExpressionSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_AssignmentExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AssignmentExpressionSyntax_System_Boolean_System_Boolean_) | Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md#_top) from the specified assignment expression\. |
| [AssignmentExpressionInfo(SyntaxNode, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_AssignmentExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_) | Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md#_top) from the specified node\. |

## AssignmentExpressionInfo\(AssignmentExpressionSyntax, Boolean, Boolean\) <a name="Roslynator_CSharp_SyntaxInfo_AssignmentExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AssignmentExpressionSyntax_System_Boolean_System_Boolean_"></a>

### Summary

Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md#_top) from the specified assignment expression\.

```csharp
public static AssignmentExpressionInfo AssignmentExpressionInfo(AssignmentExpressionSyntax assignmentExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**assignmentExpression**

**walkDownParentheses**

**allowMissing**

### Returns

Roslynator\.CSharp\.Syntax\.[AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md#_top)

## AssignmentExpressionInfo\(SyntaxNode, Boolean, Boolean\) <a name="Roslynator_CSharp_SyntaxInfo_AssignmentExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_"></a>

### Summary

Creates a new [AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md#_top) from the specified node\.

```csharp
public static AssignmentExpressionInfo AssignmentExpressionInfo(SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node**

**walkDownParentheses**

**allowMissing**

### Returns

Roslynator\.CSharp\.Syntax\.[AssignmentExpressionInfo](../../Syntax/AssignmentExpressionInfo/README.md#_top)

