<a name="_top"></a>

# SymbolExtensions\.ToMinimalTypeSyntax Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SymbolExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ToMinimalTypeSyntax(INamespaceOrTypeSymbol, SemanticModel, Int32, SymbolDisplayFormat)](#Roslynator_CSharp_SymbolExtensions_ToMinimalTypeSyntax_Microsoft_CodeAnalysis_INamespaceOrTypeSymbol_Microsoft_CodeAnalysis_SemanticModel_System_Int32_Microsoft_CodeAnalysis_SymbolDisplayFormat_) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace or type symbol |
| [ToMinimalTypeSyntax(INamespaceSymbol, SemanticModel, Int32, SymbolDisplayFormat)](#Roslynator_CSharp_SymbolExtensions_ToMinimalTypeSyntax_Microsoft_CodeAnalysis_INamespaceSymbol_Microsoft_CodeAnalysis_SemanticModel_System_Int32_Microsoft_CodeAnalysis_SymbolDisplayFormat_) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace symbol\. |
| [ToMinimalTypeSyntax(ITypeSymbol, SemanticModel, Int32, SymbolDisplayFormat)](#Roslynator_CSharp_SymbolExtensions_ToMinimalTypeSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SemanticModel_System_Int32_Microsoft_CodeAnalysis_SymbolDisplayFormat_) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\. |

## ToMinimalTypeSyntax\(INamespaceOrTypeSymbol, SemanticModel, Int32, SymbolDisplayFormat\) <a name="Roslynator_CSharp_SymbolExtensions_ToMinimalTypeSyntax_Microsoft_CodeAnalysis_INamespaceOrTypeSymbol_Microsoft_CodeAnalysis_SemanticModel_System_Int32_Microsoft_CodeAnalysis_SymbolDisplayFormat_"></a>

### Summary

Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace or type symbol

```csharp
public static TypeSyntax ToMinimalTypeSyntax(this INamespaceOrTypeSymbol namespaceOrTypeSymbol, SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
```

### Parameters

**namespaceOrTypeSymbol**

**semanticModel**

**position**

**format**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

## ToMinimalTypeSyntax\(INamespaceSymbol, SemanticModel, Int32, SymbolDisplayFormat\) <a name="Roslynator_CSharp_SymbolExtensions_ToMinimalTypeSyntax_Microsoft_CodeAnalysis_INamespaceSymbol_Microsoft_CodeAnalysis_SemanticModel_System_Int32_Microsoft_CodeAnalysis_SymbolDisplayFormat_"></a>

### Summary

Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace symbol\.

```csharp
public static TypeSyntax ToMinimalTypeSyntax(this INamespaceSymbol namespaceSymbol, SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
```

### Parameters

**namespaceSymbol**

**semanticModel**

**position**

**format**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

## ToMinimalTypeSyntax\(ITypeSymbol, SemanticModel, Int32, SymbolDisplayFormat\) <a name="Roslynator_CSharp_SymbolExtensions_ToMinimalTypeSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SemanticModel_System_Int32_Microsoft_CodeAnalysis_SymbolDisplayFormat_"></a>

### Summary

Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\.

```csharp
public static TypeSyntax ToMinimalTypeSyntax(this ITypeSymbol typeSymbol, SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
```

### Parameters

**typeSymbol**

**semanticModel**

**position**

**format**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

