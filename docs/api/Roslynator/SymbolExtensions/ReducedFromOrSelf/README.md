<a name="_top"></a>

# SymbolExtensions\.ReducedFromOrSelf\(IMethodSymbol\) Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SymbolExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

If this method is a reduced extension method, returns the definition of extension method from which this was reduced\. Otherwise, returns this symbol\.

```csharp
public static IMethodSymbol ReducedFromOrSelf(this IMethodSymbol methodSymbol)
```

### Parameters

**methodSymbol**

### Returns

Microsoft\.CodeAnalysis\.[IMethodSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.imethodsymbol)

