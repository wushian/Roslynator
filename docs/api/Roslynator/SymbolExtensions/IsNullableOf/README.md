<a name="_top"></a>

# SymbolExtensions\.IsNullableOf Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SymbolExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsNullableOf(INamedTypeSymbol, ITypeSymbol)](#Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_INamedTypeSymbol_Microsoft_CodeAnalysis_ITypeSymbol_) | Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(INamedTypeSymbol, SpecialType)](#Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_INamedTypeSymbol_Microsoft_CodeAnalysis_SpecialType_) | Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, ITypeSymbol)](#Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_ITypeSymbol_) | Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |
| [IsNullableOf(ITypeSymbol, SpecialType)](#Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SpecialType_) | Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\. |

## IsNullableOf\(INamedTypeSymbol, ITypeSymbol\) <a name="Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_INamedTypeSymbol_Microsoft_CodeAnalysis_ITypeSymbol_"></a>

### Summary

Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this INamedTypeSymbol namedTypeSymbol, ITypeSymbol typeArgument)
```

### Parameters

**namedTypeSymbol**

**typeArgument**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsNullableOf\(INamedTypeSymbol, SpecialType\) <a name="Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_INamedTypeSymbol_Microsoft_CodeAnalysis_SpecialType_"></a>

### Summary

Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this INamedTypeSymbol namedTypeSymbol, SpecialType specialType)
```

### Parameters

**namedTypeSymbol**

**specialType**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsNullableOf\(ITypeSymbol, ITypeSymbol\) <a name="Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_ITypeSymbol_"></a>

### Summary

Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this ITypeSymbol typeSymbol, ITypeSymbol typeArgument)
```

### Parameters

**typeSymbol**

**typeArgument**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsNullableOf\(ITypeSymbol, SpecialType\) <a name="Roslynator_SymbolExtensions_IsNullableOf_Microsoft_CodeAnalysis_ITypeSymbol_Microsoft_CodeAnalysis_SpecialType_"></a>

### Summary

Returns true if the type is [Nullable\<T>](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) and it has specified type argument\.

```csharp
public static bool IsNullableOf(this ITypeSymbol typeSymbol, SpecialType specialType)
```

### Parameters

**typeSymbol**

**specialType**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

