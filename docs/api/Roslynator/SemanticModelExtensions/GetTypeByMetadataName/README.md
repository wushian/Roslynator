# SemanticModelExtensions\.GetTypeByMetadataName\(SemanticModel, String\) Method

[Home](../../../README.md)

**Containing Type**: Roslynator\.[SemanticModelExtensions](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

Returns the type within the compilation's assembly using its canonical CLR metadata name\.

```csharp
public static INamedTypeSymbol GetTypeByMetadataName(this SemanticModel semanticModel, string fullyQualifiedMetadataName)
```

### Parameters

**semanticModel**

**fullyQualifiedMetadataName**

### Returns

Microsoft\.CodeAnalysis\.[INamedTypeSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol)

