<a name="_top"></a>

# SymbolExtensions\.ImplementsInterfaceMember Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SymbolExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ImplementsInterfaceMember(ISymbol, Boolean)](#Roslynator_SymbolExtensions_ImplementsInterfaceMember_Microsoft_CodeAnalysis_ISymbol_System_Boolean_) | Returns true if the the symbol implements any interface member\. |
| [ImplementsInterfaceMember(ISymbol, INamedTypeSymbol, Boolean)](#Roslynator_SymbolExtensions_ImplementsInterfaceMember_Microsoft_CodeAnalysis_ISymbol_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Boolean_) | Returns true if the symbol implements any member of the specified interface\. |
| [ImplementsInterfaceMember\<TSymbol>(ISymbol, Boolean)](../ImplementsInterfaceMember-1/README.md#Roslynator_SymbolExtensions_ImplementsInterfaceMember__1_Microsoft_CodeAnalysis_ISymbol_System_Boolean_) | Returns true if the symbol implements any interface member\. |
| [ImplementsInterfaceMember\<TSymbol>(ISymbol, INamedTypeSymbol, Boolean)](../ImplementsInterfaceMember-1/README.md#Roslynator_SymbolExtensions_ImplementsInterfaceMember__1_Microsoft_CodeAnalysis_ISymbol_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Boolean_) | Returns true if the symbol implements any member of the specified interface\. |

## ImplementsInterfaceMember\(ISymbol, Boolean\) <a name="Roslynator_SymbolExtensions_ImplementsInterfaceMember_Microsoft_CodeAnalysis_ISymbol_System_Boolean_"></a>

### Summary

Returns true if the the symbol implements any interface member\.

```csharp
public static bool ImplementsInterfaceMember(this ISymbol symbol, bool allInterfaces = false)
```

### Parameters

**symbol**

**allInterfaces**

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## ImplementsInterfaceMember\(ISymbol, INamedTypeSymbol, Boolean\) <a name="Roslynator_SymbolExtensions_ImplementsInterfaceMember_Microsoft_CodeAnalysis_ISymbol_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Boolean_"></a>

### Summary

Returns true if the symbol implements any member of the specified interface\.

```csharp
public static bool ImplementsInterfaceMember(this ISymbol symbol, INamedTypeSymbol interfaceSymbol, bool allInterfaces = false)
```

### Parameters

**symbol**

**interfaceSymbol**

**allInterfaces**

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## ImplementsInterfaceMember\<TSymbol>\(ISymbol, Boolean\) <a name="Roslynator_SymbolExtensions_ImplementsInterfaceMember__1_Microsoft_CodeAnalysis_ISymbol_System_Boolean_"></a>

### Summary

Returns true if the symbol implements any interface member\.

```csharp
public static bool ImplementsInterfaceMember<TSymbol>(this ISymbol symbol, bool allInterfaces = false) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**symbol**

**allInterfaces**

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## ImplementsInterfaceMember\<TSymbol>\(ISymbol, INamedTypeSymbol, Boolean\) <a name="Roslynator_SymbolExtensions_ImplementsInterfaceMember__1_Microsoft_CodeAnalysis_ISymbol_Microsoft_CodeAnalysis_INamedTypeSymbol_System_Boolean_"></a>

### Summary

Returns true if the symbol implements any member of the specified interface\.

```csharp
public static bool ImplementsInterfaceMember<TSymbol>(this ISymbol symbol, INamedTypeSymbol interfaceSymbol, bool allInterfaces = false) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**symbol**

**interfaceSymbol**

**allInterfaces**

If true, use [ITypeSymbol.AllInterfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.allinterfaces), otherwise use [ITypeSymbol.Interfaces](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.itypesymbol.interfaces)\.

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

