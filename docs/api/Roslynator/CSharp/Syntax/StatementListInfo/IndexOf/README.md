<a name="_top"></a>

# StatementListInfo\.IndexOf Method

[Home](../../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.Syntax\.[StatementListInfo](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IndexOf(Func\<StatementSyntax, Boolean>)](#Roslynator_CSharp_Syntax_StatementListInfo_IndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_System_Boolean__) | Searches for a statement that matches the predicate and returns returns zero\-based index of the first occurrence in the list\. |
| [IndexOf(StatementSyntax)](#Roslynator_CSharp_Syntax_StatementListInfo_IndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_) | The index of the statement in the list\. |

## IndexOf\(Func\<StatementSyntax, Boolean>\) <a name="Roslynator_CSharp_Syntax_StatementListInfo_IndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_System_Boolean__"></a>

### Summary

Searches for a statement that matches the predicate and returns returns zero\-based index of the first occurrence in the list\.

```csharp
public int IndexOf(Func<StatementSyntax, bool> predicate)
```

### Parameters

**predicate**

### Returns

System\.[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

## IndexOf\(StatementSyntax\) <a name="Roslynator_CSharp_Syntax_StatementListInfo_IndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax_"></a>

### Summary

The index of the statement in the list\.

```csharp
public int IndexOf(StatementSyntax statement)
```

### Parameters

**statement**

### Returns

System\.[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

