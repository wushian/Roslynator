<a name="_top"></a>

# MemberDeclarationListInfo\.LastIndexOf Method

[Home](../../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.Syntax\.[MemberDeclarationListInfo](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LastIndexOf(Func\<MemberDeclarationSyntax, Boolean>)](#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_LastIndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_System_Boolean__) | Searches for a member that matches the predicate and returns returns zero\-based index of the last occurrence in the list\. |
| [LastIndexOf(MemberDeclarationSyntax)](#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_LastIndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | Searches for a member and returns zero\-based index of the last occurrence in the list\. |

## LastIndexOf\(Func\<MemberDeclarationSyntax, Boolean>\) <a name="Roslynator_CSharp_Syntax_MemberDeclarationListInfo_LastIndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_System_Boolean__"></a>

### Summary

Searches for a member that matches the predicate and returns returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(Func<MemberDeclarationSyntax, bool> predicate)
```

### Parameters

**predicate**

### Returns

System\.[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

## LastIndexOf\(MemberDeclarationSyntax\) <a name="Roslynator_CSharp_Syntax_MemberDeclarationListInfo_LastIndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

### Summary

Searches for a member and returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(MemberDeclarationSyntax member)
```

### Parameters

**member**

### Returns

System\.[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

