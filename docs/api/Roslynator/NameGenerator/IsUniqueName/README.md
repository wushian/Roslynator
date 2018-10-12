<a name="_top"></a>

# NameGenerator\.IsUniqueName Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[NameGenerator](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsUniqueName(String, IEnumerable\<String>, Boolean)](#Roslynator_NameGenerator_IsUniqueName_System_String_System_Collections_Generic_IEnumerable_System_String__System_Boolean_) | Returns true if the name is not contained in the specified list\. |
| [IsUniqueName(String, ImmutableArray\<ISymbol>, Boolean)](#Roslynator_NameGenerator_IsUniqueName_System_String_System_Collections_Immutable_ImmutableArray_Microsoft_CodeAnalysis_ISymbol__System_Boolean_) | Returns true if the name is not contained in the specified list\. [ISymbol.Name](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.name) is used to compare names\. |

## IsUniqueName\(String, IEnumerable\<String>, Boolean\) <a name="Roslynator_NameGenerator_IsUniqueName_System_String_System_Collections_Generic_IEnumerable_System_String__System_Boolean_"></a>

### Summary

Returns true if the name is not contained in the specified list\.

```csharp
public static bool IsUniqueName(string name, IEnumerable<string> reservedNames, bool isCaseSensitive = true)
```

### Parameters

**name**

**reservedNames**

**isCaseSensitive**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsUniqueName\(String, ImmutableArray\<ISymbol>, Boolean\) <a name="Roslynator_NameGenerator_IsUniqueName_System_String_System_Collections_Immutable_ImmutableArray_Microsoft_CodeAnalysis_ISymbol__System_Boolean_"></a>

### Summary

Returns true if the name is not contained in the specified list\. [ISymbol.Name](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol.name) is used to compare names\.

```csharp
public static bool IsUniqueName(string name, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true)
```

### Parameters

**name**

**symbols**

**isCaseSensitive**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

