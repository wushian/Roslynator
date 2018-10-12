# MemberDeclarationListInfo\.IndexOf Method

[Home](../../../../../README.md)

**Containing Type**: Roslynator\.CSharp\.Syntax\.[MemberDeclarationListInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IndexOf(Func\<MemberDeclarationSyntax, Boolean>)](#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_IndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_System_Boolean__) | Searches for a member that matches the predicate and returns returns zero\-based index of the first occurrence in the list\. |
| [IndexOf(MemberDeclarationSyntax)](#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_IndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | The index of the member in the list\. |

## IndexOf\(Func\<MemberDeclarationSyntax, Boolean>\) <a name="Roslynator_CSharp_Syntax_MemberDeclarationListInfo_IndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_System_Boolean__"></a>

### Summary

Searches for a member that matches the predicate and returns returns zero\-based index of the first occurrence in the list\.

```csharp
public int IndexOf(Func<MemberDeclarationSyntax, bool> predicate)
```

### Parameters

**predicate**

### Returns

System\.[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

## IndexOf\(MemberDeclarationSyntax\) <a name="Roslynator_CSharp_Syntax_MemberDeclarationListInfo_IndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

### Summary

The index of the member in the list\.

```csharp
public int IndexOf(MemberDeclarationSyntax member)
```

### Parameters

**member**

### Returns

System\.[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

