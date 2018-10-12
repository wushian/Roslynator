<a name="_top"></a>

# SyntaxInfo\.StringConcatenationExpressionInfo Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxInfo](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [StringConcatenationExpressionInfo(BinaryExpressionSyntax, SemanticModel, CancellationToken)](#Roslynator_CSharp_SyntaxInfo_StringConcatenationExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_BinaryExpressionSyntax_Microsoft_CodeAnalysis_SemanticModel_System_Threading_CancellationToken_) | Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/README.md#_top) from the specified node\. |
| [StringConcatenationExpressionInfo(ExpressionChain, SemanticModel, CancellationToken)](#Roslynator_CSharp_SyntaxInfo_StringConcatenationExpressionInfo_Roslynator_CSharp_ExpressionChain__Microsoft_CodeAnalysis_SemanticModel_System_Threading_CancellationToken_) | Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/README.md#_top) from the specified expression chain\. |
| [StringConcatenationExpressionInfo(SyntaxNode, SemanticModel, Boolean, CancellationToken)](#Roslynator_CSharp_SyntaxInfo_StringConcatenationExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_SemanticModel_System_Boolean_System_Threading_CancellationToken_) | Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/README.md#_top) from the specified node\. |

## StringConcatenationExpressionInfo\(BinaryExpressionSyntax, SemanticModel, CancellationToken\) <a name="Roslynator_CSharp_SyntaxInfo_StringConcatenationExpressionInfo_Microsoft_CodeAnalysis_CSharp_Syntax_BinaryExpressionSyntax_Microsoft_CodeAnalysis_SemanticModel_System_Threading_CancellationToken_"></a>

### Summary

Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/README.md#_top) from the specified node\.

```csharp
public static StringConcatenationExpressionInfo StringConcatenationExpressionInfo(BinaryExpressionSyntax binaryExpression, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
```

### Parameters

**binaryExpression**

**semanticModel**

**cancellationToken**

### Returns

Roslynator\.CSharp\.Syntax\.[StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/README.md#_top)

## StringConcatenationExpressionInfo\(ExpressionChain, SemanticModel, CancellationToken\) <a name="Roslynator_CSharp_SyntaxInfo_StringConcatenationExpressionInfo_Roslynator_CSharp_ExpressionChain__Microsoft_CodeAnalysis_SemanticModel_System_Threading_CancellationToken_"></a>

### Summary

Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/README.md#_top) from the specified expression chain\.

```csharp
public static StringConcatenationExpressionInfo StringConcatenationExpressionInfo(in ExpressionChain expressionChain, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
```

### Parameters

**expressionChain**

**semanticModel**

**cancellationToken**

### Returns

Roslynator\.CSharp\.Syntax\.[StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/README.md#_top)

## StringConcatenationExpressionInfo\(SyntaxNode, SemanticModel, Boolean, CancellationToken\) <a name="Roslynator_CSharp_SyntaxInfo_StringConcatenationExpressionInfo_Microsoft_CodeAnalysis_SyntaxNode_Microsoft_CodeAnalysis_SemanticModel_System_Boolean_System_Threading_CancellationToken_"></a>

### Summary

Creates a new [StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/README.md#_top) from the specified node\.

```csharp
public static StringConcatenationExpressionInfo StringConcatenationExpressionInfo(SyntaxNode node, SemanticModel semanticModel, bool walkDownParentheses = true, CancellationToken cancellationToken = default(CancellationToken))
```

### Parameters

**node**

**semanticModel**

**walkDownParentheses**

**cancellationToken**

### Returns

Roslynator\.CSharp\.Syntax\.[StringConcatenationExpressionInfo](../../Syntax/StringConcatenationExpressionInfo/README.md#_top)

