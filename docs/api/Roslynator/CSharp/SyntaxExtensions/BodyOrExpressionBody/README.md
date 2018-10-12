<a name="_top"></a>

# SyntaxExtensions\.BodyOrExpressionBody Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [BodyOrExpressionBody(AccessorDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_AccessorDeclarationSyntax_) | Returns accessor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(ConstructorDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_ConstructorDeclarationSyntax_) | Returns constructor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(ConversionOperatorDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_ConversionOperatorDeclarationSyntax_) | Returns conversion operator body or an expression body if the body is null\. |
| [BodyOrExpressionBody(DestructorDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_DestructorDeclarationSyntax_) | Returns destructor body or an expression body if the body is null\. |
| [BodyOrExpressionBody(LocalFunctionStatementSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_) | Returns local function body or an expression body if the body is null\. |
| [BodyOrExpressionBody(MethodDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_) | Returns method body or an expression body if the body is null\. |
| [BodyOrExpressionBody(OperatorDeclarationSyntax)](#Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_OperatorDeclarationSyntax_) | Returns operator body or an expression body if the body is null\. |

## BodyOrExpressionBody\(AccessorDeclarationSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_AccessorDeclarationSyntax_"></a>

### Summary

Returns accessor body or an expression body if the body is null\.

```csharp
public static CSharpSyntaxNode BodyOrExpressionBody(this AccessorDeclarationSyntax accessorDeclaration)
```

### Parameters

**accessorDeclaration**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(ConstructorDeclarationSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_ConstructorDeclarationSyntax_"></a>

### Summary

Returns constructor body or an expression body if the body is null\.

```csharp
public static CSharpSyntaxNode BodyOrExpressionBody(this ConstructorDeclarationSyntax constructorDeclaration)
```

### Parameters

**constructorDeclaration**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(ConversionOperatorDeclarationSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_ConversionOperatorDeclarationSyntax_"></a>

### Summary

Returns conversion operator body or an expression body if the body is null\.

```csharp
public static CSharpSyntaxNode BodyOrExpressionBody(this ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
```

### Parameters

**conversionOperatorDeclaration**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(DestructorDeclarationSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_DestructorDeclarationSyntax_"></a>

### Summary

Returns destructor body or an expression body if the body is null\.

```csharp
public static CSharpSyntaxNode BodyOrExpressionBody(this DestructorDeclarationSyntax destructorDeclaration)
```

### Parameters

**destructorDeclaration**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(LocalFunctionStatementSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_LocalFunctionStatementSyntax_"></a>

### Summary

Returns local function body or an expression body if the body is null\.

```csharp
public static CSharpSyntaxNode BodyOrExpressionBody(this LocalFunctionStatementSyntax localFunctionStatement)
```

### Parameters

**localFunctionStatement**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(MethodDeclarationSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_MethodDeclarationSyntax_"></a>

### Summary

Returns method body or an expression body if the body is null\.

```csharp
public static CSharpSyntaxNode BodyOrExpressionBody(this MethodDeclarationSyntax methodDeclaration)
```

### Parameters

**methodDeclaration**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

## BodyOrExpressionBody\(OperatorDeclarationSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_BodyOrExpressionBody_Microsoft_CodeAnalysis_CSharp_Syntax_OperatorDeclarationSyntax_"></a>

### Summary

Returns operator body or an expression body if the body is null\.

```csharp
public static CSharpSyntaxNode BodyOrExpressionBody(this OperatorDeclarationSyntax operatorDeclaration)
```

### Parameters

**operatorDeclaration**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.[CSharpSyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxnode)

