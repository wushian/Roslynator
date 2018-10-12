<a name="_top"></a>

# SyntaxExtensions\.ReturnsVoid Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReturnsVoid(DelegateDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_DelegateDeclarationSyntax_) | Returns true the specified delegate return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReturnsVoid(LocalFunctionStatementSyntax)](#Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_) | Returns true if the specified local function' return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |
| [ReturnsVoid(MethodDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_) | Returns true if the specified method return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\. |

## ReturnsVoid\(DelegateDeclarationSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_DelegateDeclarationSyntax_"></a>

### Summary

Returns true the specified delegate return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\.

```csharp
public static bool ReturnsVoid(this DelegateDeclarationSyntax delegateDeclaration)
```

### Parameters

**delegateDeclaration**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## ReturnsVoid\(LocalFunctionStatementSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_"></a>

### Summary

Returns true if the specified local function' return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\.

```csharp
public static bool ReturnsVoid(this LocalFunctionStatementSyntax localFunctionStatement)
```

### Parameters

**localFunctionStatement**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## ReturnsVoid\(MethodDeclarationSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_ReturnsVoid_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_"></a>

### Summary

Returns true if the specified method return type is [Void](https://docs.microsoft.com/en-us/dotnet/api/system.void)\.

```csharp
public static bool ReturnsVoid(this MethodDeclarationSyntax methodDeclaration)
```

### Parameters

**methodDeclaration**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

