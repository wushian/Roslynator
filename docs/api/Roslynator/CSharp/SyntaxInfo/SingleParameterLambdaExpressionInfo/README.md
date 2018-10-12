<a name="_top"></a>

# SyntaxInfo\.SingleParameterLambdaExpressionInfo Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxInfo](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [SingleParameterLambdaExpressionInfo(LambdaExpressionSyntax, Boolean)](#Roslynator_CSharp_SyntaxInfo_SingleParameterLambdaExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_LambdaExpressionSyntax_System_Boolean_) | Creates a new [SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/README.md#_top) from the specified lambda expression\. |
| [SingleParameterLambdaExpressionInfo(SyntaxNode, Boolean, Boolean)](#Roslynator_CSharp_SyntaxInfo_SingleParameterLambdaExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_) | Creates a new [SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/README.md#_top) from the specified node\. |

## SingleParameterLambdaExpressionInfo\(LambdaExpressionSyntax, Boolean\) <a name="Roslynator_CSharp_SyntaxInfo_SingleParameterLambdaExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_LambdaExpressionSyntax_System_Boolean_"></a>

### Summary

Creates a new [SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/README.md#_top) from the specified lambda expression\.

```csharp
public static SingleParameterLambdaExpressionInfo SingleParameterLambdaExpressionInfo(LambdaExpressionSyntax lambdaExpression, bool allowMissing = false)
```

### Parameters

**lambdaExpression**

**allowMissing**

### Returns

Roslynator\.CSharp\.Syntax\.[SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/README.md#_top)

## SingleParameterLambdaExpressionInfo\(SyntaxNode, Boolean, Boolean\) <a name="Roslynator_CSharp_SyntaxInfo_SingleParameterLambdaExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_System_Boolean_System_Boolean_"></a>

### Summary

Creates a new [SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/README.md#_top) from the specified node\.

```csharp
public static SingleParameterLambdaExpressionInfo SingleParameterLambdaExpressionInfo(SyntaxNode node, bool walkDownParentheses = true, bool allowMissing = false)
```

### Parameters

**node**

**walkDownParentheses**

**allowMissing**

### Returns

Roslynator\.CSharp\.Syntax\.[SingleParameterLambdaExpressionInfo](../../Syntax/SingleParameterLambdaExpressionInfo/README.md#_top)

