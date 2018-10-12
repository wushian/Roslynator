<a name="_top"></a>

# SymbolExtensions\.ToTypeSyntax Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SymbolExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ToTypeSyntax(INamespaceOrTypeSymbol, SymbolDisplayFormat)](#Roslynator_CSharp_SymbolExtensions_ToTypeSyntax_Microsoft_CodeAnalysis_INamespaceOrTypeSymbol_Microsoft_CodeAnalysis_SymbolDisplayFormat_) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace or type symbol\. |
| [ToTypeSyntax(INamespaceSymbol, SymbolDisplayFormat)](#Roslynator_CSharp_SymbolExtensions_ToTypeSyntax_Microsoft_CodeAnalysis_INamespaceSymbol_Microsoft_CodeAnalysis_SymbolDisplayFormat_) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace symbol\. |
| [ToTypeSyntax(ITypeSymbol, SymbolDisplayFormat)](#Roslynator_CSharp_SymbolExtensions_ToTypeSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SymbolDisplayFormat_) | Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\. |

## ToTypeSyntax\(INamespaceOrTypeSymbol, SymbolDisplayFormat\) <a name="Roslynator_CSharp_SymbolExtensions_ToTypeSyntax_Microsoft_CodeAnalysis_INamespaceOrTypeSymbol_Microsoft_CodeAnalysis_SymbolDisplayFormat_"></a>

### Summary

Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace or type symbol\.

```csharp
public static TypeSyntax ToTypeSyntax(this INamespaceOrTypeSymbol namespaceOrTypeSymbol, SymbolDisplayFormat format = null)
```

### Parameters

**namespaceOrTypeSymbol**

**format**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

## ToTypeSyntax\(INamespaceSymbol, SymbolDisplayFormat\) <a name="Roslynator_CSharp_SymbolExtensions_ToTypeSyntax_Microsoft_CodeAnalysis_INamespaceSymbol_Microsoft_CodeAnalysis_SymbolDisplayFormat_"></a>

### Summary

Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified namespace symbol\.

```csharp
public static TypeSyntax ToTypeSyntax(this INamespaceSymbol namespaceSymbol, SymbolDisplayFormat format = null)
```

### Parameters

**namespaceSymbol**

**format**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

## ToTypeSyntax\(ITypeSymbol, SymbolDisplayFormat\) <a name="Roslynator_CSharp_SymbolExtensions_ToTypeSyntax_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SymbolDisplayFormat_"></a>

### Summary

Creates a new [TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax) based on the specified type symbol\.

```csharp
public static TypeSyntax ToTypeSyntax(this ITypeSymbol typeSymbol, SymbolDisplayFormat format = null)
```

### Parameters

**typeSymbol**

**format**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[TypeSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.typesyntax)

