<a name="_top"></a>

# SymbolExtensions\.IsParameterArrayOf Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SymbolExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsParameterArrayOf(IParameterSymbol, SpecialType)](#Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_) | Returns true if the parameter was declared as a parameter array that has a specified element type\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType, SpecialType)](#Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_) | Returns true if the parameter was declared as a parameter array that has one of specified element types\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType, SpecialType, SpecialType)](#Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_) | Returns true if the parameter was declared as a parameter array that has one of specified element types\. |

## IsParameterArrayOf\(IParameterSymbol, SpecialType\) <a name="Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_"></a>

### Summary

Returns true if the parameter was declared as a parameter array that has a specified element type\.

```csharp
public static bool IsParameterArrayOf(this IParameterSymbol parameterSymbol, SpecialType elementType)
```

### Parameters

**parameterSymbol**

**elementType**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParameterArrayOf\(IParameterSymbol, SpecialType, SpecialType\) <a name="Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_"></a>

### Summary

Returns true if the parameter was declared as a parameter array that has one of specified element types\.

```csharp
public static bool IsParameterArrayOf(this IParameterSymbol parameterSymbol, SpecialType elementType1, SpecialType elementType2)
```

### Parameters

**parameterSymbol**

**elementType1**

**elementType2**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParameterArrayOf\(IParameterSymbol, SpecialType, SpecialType, SpecialType\) <a name="Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_"></a>

### Summary

Returns true if the parameter was declared as a parameter array that has one of specified element types\.

```csharp
public static bool IsParameterArrayOf(this IParameterSymbol parameterSymbol, SpecialType elementType1, SpecialType elementType2, SpecialType elementType3)
```

### Parameters

**parameterSymbol**

**elementType1**

**elementType2**

**elementType3**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

