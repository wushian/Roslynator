<a name="_top"></a>

# MemberDeclarationListSelection\.TryCreate Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[MemberDeclarationListSelection](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [TryCreate(NamespaceDeclarationSyntax, TextSpan, MemberDeclarationListSelection)](#Roslynator_CSharp_MemberDeclarationListSelection_TryCreate_Microsoft_CodeAnalysis_CSharp_Syntax_NamespaceDeclarationSyntax_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_MemberDeclarationListSelection__) | Creates a new [MemberDeclarationListSelection](../README.md#_top) based on the specified namespace declaration and span\. |
| [TryCreate(TypeDeclarationSyntax, TextSpan, MemberDeclarationListSelection)](#Roslynator_CSharp_MemberDeclarationListSelection_TryCreate_Microsoft_CodeAnalysis_CSharp_Syntax_TypeDeclarationSyntax_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_MemberDeclarationListSelection__) | Creates a new [MemberDeclarationListSelection](../README.md#_top) based on the specified type declaration and span\. |

## TryCreate\(NamespaceDeclarationSyntax, TextSpan, MemberDeclarationListSelection\) <a name="Roslynator_CSharp_MemberDeclarationListSelection_TryCreate_Microsoft_CodeAnalysis_CSharp_Syntax_NamespaceDeclarationSyntax_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_MemberDeclarationListSelection__"></a>

### Summary

Creates a new [MemberDeclarationListSelection](../README.md#_top) based on the specified namespace declaration and span\.

```csharp
public static bool TryCreate(NamespaceDeclarationSyntax namespaceDeclaration, TextSpan span, out MemberDeclarationListSelection selectedMembers)
```

### Parameters

**namespaceDeclaration**

**span**

**selectedMembers**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one member; otherwise, false\.

## TryCreate\(TypeDeclarationSyntax, TextSpan, MemberDeclarationListSelection\) <a name="Roslynator_CSharp_MemberDeclarationListSelection_TryCreate_Microsoft_CodeAnalysis_CSharp_Syntax_TypeDeclarationSyntax_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_MemberDeclarationListSelection__"></a>

### Summary

Creates a new [MemberDeclarationListSelection](../README.md#_top) based on the specified type declaration and span\.

```csharp
public static bool TryCreate(TypeDeclarationSyntax typeDeclaration, TextSpan span, out MemberDeclarationListSelection selectedMembers)
```

### Parameters

**typeDeclaration**

**span**

**selectedMembers**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one member; otherwise, false\.