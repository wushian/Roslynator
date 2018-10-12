<a name="_top"></a>

# NameGenerator\.EnsureUniqueName Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[NameGenerator](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [EnsureUniqueName(String, IEnumerable\<String>, Boolean)](#Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Generic_IEnumerable_System_String__System_Boolean_) | Returns an unique name using the specified list of reserved names\. |
| [EnsureUniqueName(String, ImmutableArray\<ISymbol>, Boolean)](#Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Immutable_ImmutableArray_Microsoft_CodeAnalysis_ISymbol__System_Boolean_) | Returns an unique name using the specified list of symbols\. |

## EnsureUniqueName\(String, IEnumerable\<String>, Boolean\) <a name="Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Generic_IEnumerable_System_String__System_Boolean_"></a>

### Summary

Returns an unique name using the specified list of reserved names\.

```csharp
public abstract string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, bool isCaseSensitive = true)
```

### Parameters

**baseName**

**reservedNames**

**isCaseSensitive**

### Returns

System\.[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

## EnsureUniqueName\(String, ImmutableArray\<ISymbol>, Boolean\) <a name="Roslynator_NameGenerator_EnsureUniqueName_System_String_System_Collections_Immutable_ImmutableArray_Microsoft_CodeAnalysis_ISymbol__System_Boolean_"></a>

### Summary

Returns an unique name using the specified list of symbols\.

```csharp
public abstract string EnsureUniqueName(string baseName, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true)
```

### Parameters

**baseName**

**symbols**

**isCaseSensitive**

### Returns

System\.[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

