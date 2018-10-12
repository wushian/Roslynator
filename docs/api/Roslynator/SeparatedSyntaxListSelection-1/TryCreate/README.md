# SeparatedSyntaxListSelection\<TNode>\.TryCreate\(SeparatedSyntaxList\<TNode>, TextSpan, SeparatedSyntaxListSelection\<TNode>\) Method

[Home](../../../README.md)

**Containing Type**: Roslynator\.[SeparatedSyntaxListSelection\<TNode>](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

Creates a new [SeparatedSyntaxListSelection\<TNode>](../README.md) based on the specified list and span\.

```csharp
public static bool TryCreate(SeparatedSyntaxList<TNode> list, TextSpan span, out SeparatedSyntaxListSelection<TNode> selection)
```

### Parameters

**list**

**span**

**selection**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one node; otherwise, false\.