<a name="_top"></a>

# SyntaxExtensions\.All Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [All(SyntaxTokenList, Func\<SyntaxToken, Boolean>)](../All/README.md#Roslynator_SyntaxExtensions_All_Microsoft_CodeAnalysis_SyntaxTokenList_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__) | Returns true if all tokens in a [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) matches the predicate\. |
| [All(SyntaxTriviaList, Func\<SyntaxTrivia, Boolean>)](../All/README.md#Roslynator_SyntaxExtensions_All_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Func_Microsoft_CodeAnalysis_SyntaxTrivia_System_Boolean__) | Returns true if all trivia in a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) matches the predicate\. |
| [All\<TNode>(SeparatedSyntaxList\<TNode>, Func\<TNode, Boolean>)](#Roslynator_SyntaxExtensions_All__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Func___0_System_Boolean__) | Returns true if all nodes in a list matches the predicate\. |
| [All\<TNode>(SyntaxList\<TNode>, Func\<TNode, Boolean>)](#Roslynator_SyntaxExtensions_All__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Func___0_System_Boolean__) | Returns true if all nodes in a list matches the predicate\. |

## All\(SyntaxTokenList, Func\<SyntaxToken, Boolean>\) <a name="Roslynator_SyntaxExtensions_All_Microsoft_CodeAnalysis_SyntaxTokenList_System_Func_Microsoft_CodeAnalysis_SyntaxToken_System_Boolean__"></a>

### Summary

Returns true if all tokens in a [SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist) matches the predicate\.

```csharp
public static bool All(this SyntaxTokenList list, Func<SyntaxToken, bool> predicate)
```

### Parameters

**list**

**predicate**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## All\(SyntaxTriviaList, Func\<SyntaxTrivia, Boolean>\) <a name="Roslynator_SyntaxExtensions_All_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Func_Microsoft_CodeAnalysis_SyntaxTrivia_System_Boolean__"></a>

### Summary

Returns true if all trivia in a [SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist) matches the predicate\.

```csharp
public static bool All(this SyntaxTriviaList list, Func<SyntaxTrivia, bool> predicate)
```

### Parameters

**list**

**predicate**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## All\<TNode>\(SeparatedSyntaxList\<TNode>, Func\<TNode, Boolean>\) <a name="Roslynator_SyntaxExtensions_All__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Func___0_System_Boolean__"></a>

### Summary

Returns true if all nodes in a list matches the predicate\.

```csharp
public static bool All<TNode>(this SeparatedSyntaxList<TNode> list, Func<TNode, bool> predicate) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**predicate**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## All\<TNode>\(SyntaxList\<TNode>, Func\<TNode, Boolean>\) <a name="Roslynator_SyntaxExtensions_All__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Func___0_System_Boolean__"></a>

### Summary

Returns true if all nodes in a list matches the predicate\.

```csharp
public static bool All<TNode>(this SyntaxList<TNode> list, Func<TNode, bool> predicate) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**predicate**

### Returns

System\.[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

