<a name="_top"></a>

# CSharpFactory\.CollectionInitializerExpression Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[CSharpFactory](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [CollectionInitializerExpression(SeparatedSyntaxList\<ExpressionSyntax>)](#Roslynator_CSharp_CSharpFactory_CollectionInitializerExpression_Microsoft_CodeAnalysis_SeparatedSyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax__) | |
| [CollectionInitializerExpression(SyntaxToken, SeparatedSyntaxList\<ExpressionSyntax>, SyntaxToken)](#Roslynator_CSharp_CSharpFactory_CollectionInitializerExpression_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SeparatedSyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax__Microsoft_CodeAnalysis_SyntaxToken_) | |

## CollectionInitializerExpression\(SeparatedSyntaxList\<ExpressionSyntax>\) <a name="Roslynator_CSharp_CSharpFactory_CollectionInitializerExpression_Microsoft_CodeAnalysis_SeparatedSyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax__"></a>

```csharp
public static InitializerExpressionSyntax CollectionInitializerExpression(SeparatedSyntaxList<ExpressionSyntax> expressions = default(SeparatedSyntaxList<ExpressionSyntax>))
```

### Parameters

**expressions**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[InitializerExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.initializerexpressionsyntax)

## CollectionInitializerExpression\(SyntaxToken, SeparatedSyntaxList\<ExpressionSyntax>, SyntaxToken\) <a name="Roslynator_CSharp_CSharpFactory_CollectionInitializerExpression_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SeparatedSyntaxList_Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax__Microsoft_CodeAnalysis_SyntaxToken_"></a>

```csharp
public static InitializerExpressionSyntax CollectionInitializerExpression(SyntaxToken openBraceToken, SeparatedSyntaxList<ExpressionSyntax> expressions, SyntaxToken closeBraceToken)
```

### Parameters

**openBraceToken**

**expressions**

**closeBraceToken**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[InitializerExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.initializerexpressionsyntax)

