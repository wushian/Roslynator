<a name="_top"></a>

# SyntaxExtensions\.ParenthesesSpan Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ParenthesesSpan(CastExpressionSyntax)](#Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_CastExpressionSyntax_) | The absolute span of the parentheses, not including its leading and trailing trivia\. |
| [ParenthesesSpan(CommonForEachStatementSyntax)](#Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_CommonForEachStatementSyntax_) | The absolute span of the parentheses, not including its leading and trailing trivia\. |
| [ParenthesesSpan(ForStatementSyntax)](#Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_ForStatementSyntax_) | Absolute span of the parentheses, not including the leading and trailing trivia\. |

## ParenthesesSpan\(CastExpressionSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_CastExpressionSyntax_"></a>

### Summary

The absolute span of the parentheses, not including its leading and trailing trivia\.

```csharp
public static TextSpan ParenthesesSpan(this CastExpressionSyntax castExpression)
```

### Parameters

**castExpression**

### Returns

Microsoft\.CodeAnalysis\.Text\.[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

## ParenthesesSpan\(CommonForEachStatementSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_CommonForEachStatementSyntax_"></a>

### Summary

The absolute span of the parentheses, not including its leading and trailing trivia\.

```csharp
public static TextSpan ParenthesesSpan(this CommonForEachStatementSyntax forEachStatement)
```

### Parameters

**forEachStatement**

### Returns

Microsoft\.CodeAnalysis\.Text\.[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

## ParenthesesSpan\(ForStatementSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_ParenthesesSpan_Microsoft_CodeAnalysis_CSharp_Syntax_ForStatementSyntax_"></a>

### Summary

Absolute span of the parentheses, not including the leading and trailing trivia\.

```csharp
public static TextSpan ParenthesesSpan(this ForStatementSyntax forStatement)
```

### Parameters

**forStatement**

### Returns

Microsoft\.CodeAnalysis\.Text\.[TextSpan](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.text.textspan)

