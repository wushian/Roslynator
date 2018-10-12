<a name="_top"></a>

# SemanticModelExtensions\.GetEnclosingSymbol\<TSymbol>\(SemanticModel, Int32, CancellationToken\) Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SemanticModelExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

Returns the innermost symbol of type **TSymbol** that the specified position is considered inside of\.

```csharp
public static TSymbol GetEnclosingSymbol<TSymbol>(this SemanticModel semanticModel, int position, CancellationToken cancellationToken = default(CancellationToken)) where TSymbol : Microsoft.CodeAnalysis.ISymbol
```

### Type Parameters

**TSymbol**

### Parameters

**semanticModel**

**position**

**cancellationToken**

### Returns

TSymbol

