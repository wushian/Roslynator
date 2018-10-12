<a name="_top"></a>

# SyntaxExtensions\.RemoveRange Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[SyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [RemoveRange(SyntaxTokenList, Int32, Int32)](../RemoveRange/README.md#Roslynator_CSharp_SyntaxExtensions_RemoveRange_Microsoft_CodeAnalysis_SyntaxTokenList_System_Int32_System_Int32_) | Creates a new list with tokens in the specified range removed\. |
| [RemoveRange(SyntaxTriviaList, Int32, Int32)](../RemoveRange/README.md#Roslynator_CSharp_SyntaxExtensions_RemoveRange_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Int32_System_Int32_) | Creates a new list with trivia in the specified range removed\. |
| [RemoveRange\<TNode>(SeparatedSyntaxList\<TNode>, Int32, Int32)](#Roslynator_CSharp_SyntaxExtensions_RemoveRange__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32_System_Int32_) | Creates a new list with elements in the specified range removed\. |
| [RemoveRange\<TNode>(SyntaxList\<TNode>, Int32, Int32)](#Roslynator_CSharp_SyntaxExtensions_RemoveRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32_) | Creates a new list with elements in the specified range removed\. |

## RemoveRange\(SyntaxTokenList, Int32, Int32\) <a name="Roslynator_CSharp_SyntaxExtensions_RemoveRange_Microsoft_CodeAnalysis_SyntaxTokenList_System_Int32_System_Int32_"></a>

### Summary

Creates a new list with tokens in the specified range removed\.

```csharp
public static SyntaxTokenList RemoveRange(this SyntaxTokenList list, int index, int count)
```

### Parameters

**list**

**index**

An index of the first element to remove\.

**count**

A number of elements to remove\.

### Returns

Microsoft\.CodeAnalysis\.[SyntaxTokenList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtokenlist)

## RemoveRange\(SyntaxTriviaList, Int32, Int32\) <a name="Roslynator_CSharp_SyntaxExtensions_RemoveRange_Microsoft_CodeAnalysis_SyntaxTriviaList_System_Int32_System_Int32_"></a>

### Summary

Creates a new list with trivia in the specified range removed\.

```csharp
public static SyntaxTriviaList RemoveRange(this SyntaxTriviaList list, int index, int count)
```

### Parameters

**list**

**index**

An index of the first element to remove\.

**count**

A number of elements to remove\.

### Returns

Microsoft\.CodeAnalysis\.[SyntaxTriviaList](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivialist)

## RemoveRange\<TNode>\(SeparatedSyntaxList\<TNode>, Int32, Int32\) <a name="Roslynator_CSharp_SyntaxExtensions_RemoveRange__1_Microsoft_CodeAnalysis_SeparatedSyntaxList___0__System_Int32_System_Int32_"></a>

### Summary

Creates a new list with elements in the specified range removed\.

```csharp
public static SeparatedSyntaxList<TNode> RemoveRange<TNode>(this SeparatedSyntaxList<TNode> list, int index, int count) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**index**

An index of the first element to remove\.

**count**

A number of elements to remove\.

### Returns

Microsoft\.CodeAnalysis\.[SeparatedSyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.separatedsyntaxlist-1)

## RemoveRange\<TNode>\(SyntaxList\<TNode>, Int32, Int32\) <a name="Roslynator_CSharp_SyntaxExtensions_RemoveRange__1_Microsoft_CodeAnalysis_SyntaxList___0__System_Int32_System_Int32_"></a>

### Summary

Creates a new list with elements in the specified range removed\.

```csharp
public static SyntaxList<TNode> RemoveRange<TNode>(this SyntaxList<TNode> list, int index, int count) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**list**

**index**

An index of the first element to remove\.

**count**

A number of elements to remove\.

### Returns

Microsoft\.CodeAnalysis\.[SyntaxList\<TNode>](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1)

