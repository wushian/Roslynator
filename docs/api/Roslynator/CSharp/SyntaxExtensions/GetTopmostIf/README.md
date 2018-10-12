<a name="_top"></a>

# SyntaxExtensions\.GetTopmostIf Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetTopmostIf(ElseClauseSyntax)](#Roslynator_CSharp_SyntaxExtensions_GetTopmostIf_Microsoft_CodeAnalysis_CSharp_Syntax_ElseClauseSyntax_) | Returns topmost if statement of the if\-else cascade the specified else clause is part of\. |
| [GetTopmostIf(IfStatementSyntax)](#Roslynator_CSharp_SyntaxExtensions_GetTopmostIf_Microsoft_CodeAnalysis_CSharp_Syntax_IfStatementSyntax_) | Returns topmost if statement of the if\-else cascade the specified if statement is part of\. |

## GetTopmostIf\(ElseClauseSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_GetTopmostIf_Microsoft_CodeAnalysis_CSharp_Syntax_ElseClauseSyntax_"></a>

### Summary

Returns topmost if statement of the if\-else cascade the specified else clause is part of\.

```csharp
public static IfStatementSyntax GetTopmostIf(this ElseClauseSyntax elseClause)
```

### Parameters

**elseClause**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

## GetTopmostIf\(IfStatementSyntax\) <a name="Roslynator_CSharp_SyntaxExtensions_GetTopmostIf_Microsoft_CodeAnalysis_CSharp_Syntax_IfStatementSyntax_"></a>

### Summary

Returns topmost if statement of the if\-else cascade the specified if statement is part of\.

```csharp
public static IfStatementSyntax GetTopmostIf(this IfStatementSyntax ifStatement)
```

### Parameters

**ifStatement**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[IfStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.ifstatementsyntax)

