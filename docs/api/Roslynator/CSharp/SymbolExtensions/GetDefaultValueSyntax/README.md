<a name="_top"></a>

# SymbolExtensions\.GetDefaultValueSyntax Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SymbolExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetDefaultValueSyntax(ITypeSymbol, SemanticModel, Int32, SymbolDisplayFormat)](#Roslynator_CSharp_SymbolExtensions_GetDefaultValueSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SemanticModel_System_Int32_Microsoft_CodeAnalysis_SymbolDisplayFormat_) | Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\. |
| [GetDefaultValueSyntax(ITypeSymbol, TypeSyntax)](#Roslynator_CSharp_SymbolExtensions_GetDefaultValueSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_) | Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\. |

## GetDefaultValueSyntax\(ITypeSymbol, SemanticModel, Int32, SymbolDisplayFormat\) <a name="Roslynator_CSharp_SymbolExtensions_GetDefaultValueSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SemanticModel_System_Int32_Microsoft_CodeAnalysis_SymbolDisplayFormat_"></a>

### Summary

Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\.

```csharp
public static ExpressionSyntax GetDefaultValueSyntax(this ITypeSymbol typeSymbol, SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
```

### Parameters

**typeSymbol**

**semanticModel**

**position**

**format**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

## GetDefaultValueSyntax\(ITypeSymbol, TypeSyntax\) <a name="Roslynator_CSharp_SymbolExtensions_GetDefaultValueSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax_"></a>

### Summary

Creates a new [ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax) that represents default value of the specified type symbol\.

```csharp
public static ExpressionSyntax GetDefaultValueSyntax(this ITypeSymbol typeSymbol, TypeSyntax type)
```

### Parameters

**typeSymbol**

**type**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[ExpressionSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.expressionsyntax)

