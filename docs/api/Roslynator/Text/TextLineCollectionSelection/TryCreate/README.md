<a name="_top"></a>

# TextLineCollectionSelection\.TryCreate\(TextLineCollection, TextSpan, TextLineCollectionSelection\) Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.Text\.[TextLineCollectionSelection](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

Creates a new [TextLineCollectionSelection](../README.md#_top) based on the specified list and span\.

```csharp
public static bool TryCreate(TextLineCollection lines, TextSpan span, out TextLineCollectionSelection selectedLines)
```

### Parameters

**lines**

**span**

**selectedLines**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

True if the specified span contains at least one line; otherwise, false\.