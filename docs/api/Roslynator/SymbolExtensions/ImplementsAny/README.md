<a name="_top"></a>

# SymbolExtensions\.ImplementsAny Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SymbolExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, Boolean)](#Roslynator_SymbolExtensions_ImplementsAny_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_System_Boolean_) | Returns true if the type implements any of specified interfaces\. |
| [ImplementsAny(ITypeSymbol, SpecialType, SpecialType, SpecialType, Boolean)](#Roslynator_SymbolExtensions_ImplementsAny_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_System_Boolean_) | Returns true if the type implements any of specified interfaces\. |

## ImplementsAny\(ITypeSymbol, SpecialType, SpecialType, Boolean\) <a name="Roslynator_SymbolExtensions_ImplementsAny_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_System_Boolean_"></a>

### Summary

Returns true if the type implements any of specified interfaces\.

```csharp
public static bool ImplementsAny(this ITypeSymbol typeSymbol, SpecialType interfaceType1, SpecialType interfaceType2, bool allInterfaces = false)
```

### Parameters

**typeSymbol**

**interfaceType1**

**interfaceType2**

**allInterfaces**

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## ImplementsAny\(ITypeSymbol, SpecialType, SpecialType, SpecialType, Boolean\) <a name="Roslynator_SymbolExtensions_ImplementsAny_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_System_Boolean_"></a>

### Summary

Returns true if the type implements any of specified interfaces\.

```csharp
public static bool ImplementsAny(this ITypeSymbol typeSymbol, SpecialType interfaceType1, SpecialType interfaceType2, SpecialType interfaceType3, bool allInterfaces = false)
```

### Parameters

**typeSymbol**

**interfaceType1**

**interfaceType2**

**interfaceType3**

**allInterfaces**

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

