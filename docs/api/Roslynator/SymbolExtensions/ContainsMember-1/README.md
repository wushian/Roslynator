<a name="_top"></a>

# SymbolExtensions\.ContainsMember Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SymbolExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ContainsMember\<TSymbol>(ITypeSymbol, Func\<TSymbol, Boolean>)](#Roslynator_SymbolExtensions_ContainsMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_Func___0_System_Boolean__) | Returns true if the type contains member that matches the conditions defined by the specified predicate, if any\. |
| [ContainsMember\<TSymbol>(ITypeSymbol, String, Func\<TSymbol, Boolean>)](#Roslynator_SymbolExtensions_ContainsMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_String_System_Func___0_System_Boolean__) | Returns true if the type contains member that has the specified name and matches the conditions defined by the specified predicate, if any\. |

## ContainsMember\<TSymbol>\(ITypeSymbol, Func\<TSymbol, Boolean>\) <a name="Roslynator_SymbolExtensions_ContainsMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_Func___0_System_Boolean__"></a>

### Summary

Returns true if the type contains member that matches the conditions defined by the specified predicate, if any\.

```csharp
public static bool ContainsMember<TSymbol>(this ITypeSymbol typeSymbol, Func<TSymbol, bool> predicate = null) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol**

**predicate**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## ContainsMember\<TSymbol>\(ITypeSymbol, String, Func\<TSymbol, Boolean>\) <a name="Roslynator_SymbolExtensions_ContainsMember__1_Microsoft_CodeAnalysis_ITypeSymbol_System_String_System_Func___0_System_Boolean__"></a>

### Summary

Returns true if the type contains member that has the specified name and matches the conditions defined by the specified predicate, if any\.

```csharp
public static bool ContainsMember<TSymbol>(this ITypeSymbol typeSymbol, string name, Func<TSymbol, bool> predicate = null) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**typeSymbol**

**name**

**predicate**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

