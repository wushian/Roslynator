<a name="_top"></a>

# UsingDirectiveListInfo\.LastIndexOf Method

[Home](../../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.Syntax\.[UsingDirectiveListInfo](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LastIndexOf(Func\<UsingDirectiveSyntax, Boolean>)](#Roslynator_CSharp_Syntax_UsingDirectiveListInfo_LastIndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax_System_Boolean__) | Searches for an using directive that matches the predicate and returns returns zero\-based index of the last occurrence in the list\. |
| [LastIndexOf(UsingDirectiveSyntax)](#Roslynator_CSharp_Syntax_UsingDirectiveListInfo_LastIndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax_) | Searches for an using directive and returns zero\-based index of the last occurrence in the list\. |

## LastIndexOf\(Func\<UsingDirectiveSyntax, Boolean>\) <a name="Roslynator_CSharp_Syntax_UsingDirectiveListInfo_LastIndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax_System_Boolean__"></a>

### Summary

Searches for an using directive that matches the predicate and returns returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(Func<UsingDirectiveSyntax, bool> predicate)
```

### Parameters

**predicate**

### Returns

System\.[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

## LastIndexOf\(UsingDirectiveSyntax\) <a name="Roslynator_CSharp_Syntax_UsingDirectiveListInfo_LastIndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_UsingDirectiveSyntax_"></a>

### Summary

Searches for an using directive and returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(UsingDirectiveSyntax usingDirective)
```

### Parameters

**usingDirective**

### Returns

System\.[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

