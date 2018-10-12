<a name="_top"></a>

# SyntaxExtensions\.TryGetContainingList\(SyntaxTrivia, SyntaxTriviaList, Boolean, Boolean\) Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

Gets a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) the specified trivia is contained in\.

```csharp
public static bool TryGetContainingList(this SyntaxTrivia trivia, out SyntaxTriviaList triviaList, bool allowLeading = true, bool allowTrailing = true)
```

### Parameters

**trivia**

**triviaList**

**allowLeading**

If true, trivia can be part of leading trivia\.

**allowTrailing**

If true, trivia can be part of trailing trivia\.

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

