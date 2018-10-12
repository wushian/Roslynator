<a name="_top"></a>

# NameGenerator\.EnsureUniqueMemberName Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[NameGenerator](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [EnsureUniqueMemberName(String, INamedTypeSymbol, Boolean)](#Roslynator_NameGenerator_EnsureUniqueMemberName_System_String_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Boolean_) | |
| [EnsureUniqueMemberName(String, SemanticModel, Int32, Boolean, CancellationToken)](#Roslynator_NameGenerator_EnsureUniqueMemberName_System_String_Microsoft_CodeAnalysis_SemanticModel_System_Int32_System_Boolean_System_Threading_CancellationToken_) | Returns a member name that will be unique at the specified position\. |

## EnsureUniqueMemberName\(String, INamedTypeSymbol, Boolean\) <a name="Roslynator_NameGenerator_EnsureUniqueMemberName_System_String_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Boolean_"></a>

```csharp
public string EnsureUniqueMemberName(string baseName, INamedTypeSymbol typeSymbol, bool isCaseSensitive = true)
```

### Parameters

**baseName**

**typeSymbol**

**isCaseSensitive**

### Returns

System\.[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

## EnsureUniqueMemberName\(String, SemanticModel, Int32, Boolean, CancellationToken\) <a name="Roslynator_NameGenerator_EnsureUniqueMemberName_System_String_Microsoft_CodeAnalysis_SemanticModel_System_Int32_System_Boolean_System_Threading_CancellationToken_"></a>

### Summary

Returns a member name that will be unique at the specified position\.

```csharp
public string EnsureUniqueMemberName(string baseName, SemanticModel semanticModel, int position, bool isCaseSensitive = true, CancellationToken cancellationToken = default(CancellationToken))
```

### Parameters

**baseName**

**semanticModel**

**position**

**isCaseSensitive**

**cancellationToken**

### Returns

System\.[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

