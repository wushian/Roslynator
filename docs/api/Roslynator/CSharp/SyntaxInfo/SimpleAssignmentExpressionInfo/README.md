<a name="_top"></a>

# SyntaxInfo\.SimpleAssignmentExpressionInfo Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxInfo](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SimpleAssignmentExpressionInfo(AssignmentExpressionSyntax, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_SimpleAssignmentExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AssignmentExpressionSyntax_System_Boolean_System_Boolean_) | Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md#_top) from the specified assignment expression\. |
| [SimpleAssignmentExpressionInfo(SyntaxNode, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_SimpleAssignmentExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_) | Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md#_top) from the specified node\. |

## SimpleAssignmentExpressionInfo\(AssignmentExpressionSyntax, Boolean, Boolean\) <a name="Roslynator_CSharp_SyntaxInfo_SimpleAssignmentExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_AssignmentExpressionSyntax_System_Boolean_System_Boolean_"></a>

### Summary

Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md#_top) from the specified assignment expression\.

```csharp
public static SimpleAssignmentExpressionInfo SimpleAssignmentExpressionInfo(AssignmentExpressionSyntax assignmentExpression, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**assignmentExpression**

**walkDownParentheses**

**allowMissing**

### Returns

Roslynator\.CSharp\.Syntax\.[SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md#_top)

## SimpleAssignmentExpressionInfo\(SyntaxNode, Boolean, Boolean\) <a name="Roslynator_CSharp_SyntaxInfo_SimpleAssignmentExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_"></a>

### Summary

Creates a new [SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md#_top) from the specified node\.

```csharp
public static SimpleAssignmentExpressionInfo SimpleAssignmentExpressionInfo(SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node**

**walkDownParentheses**

**allowMissing**

### Returns

Roslynator\.CSharp\.Syntax\.[SimpleAssignmentExpressionInfo](../../Syntax/SimpleAssignmentExpressionInfo/README.md#_top)

