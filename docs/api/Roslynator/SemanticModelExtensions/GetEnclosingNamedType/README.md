<a name="_top"></a>

# SemanticModelExtensions\.GetEnclosingNamedType\(SemanticModel, Int32, CancellationToken\) Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SemanticModelExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

Returns the innermost named type symbol that the specified position is considered inside of\.

```csharp
public static INamedTypeSymbol GetEnclosingNamedType(this SemanticModel semanticModel, int position, CancellationToken cancellationToken = default(CancellationToken))
```

### Parameters

**semanticModel**

**position**

**cancellationToken**

### Returns

Microsoft\.CodeAnalysis\.[INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

