<a name="_top"></a>

# SyntaxExtensions\.IsSimpleIf\(IfStatementSyntax\) Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

Returns true if the specified if statement is a simple if statement\.
Simple if statement is defined as follows: it is not a child of an else clause and it has no else clause\.

```csharp
public static bool IsSimpleIf(this IfStatementSyntax ifStatement)
```

### Parameters

**ifStatement**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

