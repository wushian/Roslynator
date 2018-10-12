<a name="_top"></a>

# SymbolExtensions\.Implements Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SymbolExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Implements(ITypeSymbol, INamedTypeSymbol, Boolean)](#Roslynator_SymbolExtensions_Implements_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Boolean_) | Returns true if the type implements specified interface\. |
| [Implements(ITypeSymbol, SpecialType, Boolean)](#Roslynator_SymbolExtensions_Implements_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SpecialType_System_Boolean_) | Returns true if the type implements specified interface\. |

## Implements\(ITypeSymbol, INamedTypeSymbol, Boolean\) <a name="Roslynator_SymbolExtensions_Implements_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Boolean_"></a>

### Summary

Returns true if the type implements specified interface\.

```csharp
public static bool Implements(this ITypeSymbol typeSymbol, INamedTypeSymbol interfaceSymbol, bool allInterfaces = false)
```

### Parameters

**typeSymbol**

**interfaceSymbol**

**allInterfaces**

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## Implements\(ITypeSymbol, SpecialType, Boolean\) <a name="Roslynator_SymbolExtensions_Implements_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SpecialType_System_Boolean_"></a>

### Summary

Returns true if the type implements specified interface\.

```csharp
public static bool Implements(this ITypeSymbol typeSymbol, SpecialType interfaceType, bool allInterfaces = false)
```

### Parameters

**typeSymbol**

**interfaceType**

**allInterfaces**

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

