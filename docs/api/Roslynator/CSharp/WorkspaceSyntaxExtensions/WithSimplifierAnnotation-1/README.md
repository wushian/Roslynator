<a name="_top"></a>

# WorkspaceSyntaxExtensions\.WithSimplifierAnnotation Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[WorkspaceSyntaxExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [WithSimplifierAnnotation(SyntaxToken)](../WithSimplifierAnnotation/README.md#Roslynator_CSharp_WorkspaceSyntaxExtensions_WithSimplifierAnnotation_Microsoft_CodeAnalysis_SyntaxToken_) | Adds [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) to the specified token, creating a new token of the same type with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) on it\. "Rename" annotation is specified by [RenameAnnotation.Kind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.codeactions.renameannotation.kind)\. |
| [WithSimplifierAnnotation\<TNode>(TNode)](#Roslynator_CSharp_WorkspaceSyntaxExtensions_WithSimplifierAnnotation__1___0_) | Creates a new node with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) attached\. |

## WithSimplifierAnnotation\(SyntaxToken\) <a name="Roslynator_CSharp_WorkspaceSyntaxExtensions_WithSimplifierAnnotation_Microsoft_CodeAnalysis_SyntaxToken_"></a>

### Summary

Adds [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) to the specified token, creating a new token of the same type with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) on it\.
"Rename" annotation is specified by [RenameAnnotation.Kind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.codeactions.renameannotation.kind)\.

```csharp
public static SyntaxToken WithSimplifierAnnotation(this SyntaxToken token)
```

### Parameters

**token**

### Returns

Microsoft\.CodeAnalysis\.[SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

## WithSimplifierAnnotation\<TNode>\(TNode\) <a name="Roslynator_CSharp_WorkspaceSyntaxExtensions_WithSimplifierAnnotation__1___0_"></a>

### Summary

Creates a new node with the [Simplifier.Annotation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.simplification.simplifier.annotation) attached\.

```csharp
public static TNode WithSimplifierAnnotation<TNode>(this TNode node) where TNode : Microsoft.CodeAnalysis.SyntaxNode
```

### Type Parameters

**TNode**

### Parameters

**node**

### Returns

TNode

