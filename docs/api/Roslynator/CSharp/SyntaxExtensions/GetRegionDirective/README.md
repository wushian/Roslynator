<a name="_top"></a>

# SyntaxExtensions\.GetRegionDirective\(EndRegionDirectiveTriviaSyntax\) Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

Returns region directive that is related to the specified endregion directive\. Returns null if no matching region directive is found\.

```csharp
public static RegionDirectiveTriviaSyntax GetRegionDirective(this EndRegionDirectiveTriviaSyntax endRegionDirective)
```

### Parameters

**endRegionDirective**

### Returns

Microsoft\.CodeAnalysis\.CSharp\.Syntax\.[RegionDirectiveTriviaSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.regiondirectivetriviasyntax)

