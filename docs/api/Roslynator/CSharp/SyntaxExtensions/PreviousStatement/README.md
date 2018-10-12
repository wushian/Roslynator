<a name="_top"></a>

# SyntaxExtensions\.PreviousStatement\(StatementSyntax\) Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

Gets the previous statement of the specified statement\.
If the specified statement is not contained in the list, or if there is no previous statement, then this method returns null\.

```csharp
public static StatementSyntax PreviousStatement(this StatementSyntax statement)
```

### Parameters

**statement**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[StatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.statementsyntax)

