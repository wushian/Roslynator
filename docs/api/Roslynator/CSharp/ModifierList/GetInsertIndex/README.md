<a name="_top"></a>

# ModifierList\.GetInsertIndex Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[ModifierList](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [GetInsertIndex(SyntaxTokenList, SyntaxKind, IComparer\<SyntaxKind>)](#Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__) | Returns an index a token with the specified kind should be inserted at\. |
| [GetInsertIndex(SyntaxTokenList, SyntaxToken, IComparer\<SyntaxToken>)](#Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__) | Returns an index the specified token should be inserted at\. |

## GetInsertIndex\(SyntaxTokenList, SyntaxKind, IComparer\<SyntaxKind>\) <a name="Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_CSharp_SyntaxKind_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_CSharp_SyntaxKind__"></a>

### Summary

Returns an index a token with the specified kind should be inserted at\.

```csharp
public static int GetInsertIndex(SyntaxTokenList tokens, SyntaxKind kind, IComparer<SyntaxKind> comparer = null)
```

### Parameters

**tokens**

**kind**

**comparer**

### Returns

System\.[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

## GetInsertIndex\(SyntaxTokenList, SyntaxToken, IComparer\<SyntaxToken>\) <a name="Roslynator_CSharp_ModifierList_GetInsertIndex_Microsoft_CodeAnalysis_SyntaxTokenList_Microsoft_CodeAnalysis_SyntaxToken_System_Collections_Generic_IComparer_Microsoft_CodeAnalysis_SyntaxToken__"></a>

### Summary

Returns an index the specified token should be inserted at\.

```csharp
public static int GetInsertIndex(SyntaxTokenList tokens, SyntaxToken token, IComparer<SyntaxToken> comparer = null)
```

### Parameters

**tokens**

**token**

**comparer**

### Returns

System\.[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

