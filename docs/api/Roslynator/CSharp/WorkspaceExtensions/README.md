<a name="_top"></a>

# WorkspaceExtensions Class

[Home](../../../README.md#_top) &#x2022; [Methods](#methods)

**Namespace**: [Roslynator.CSharp](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

## Summary

A set of extension methods for the workspace layer\.

```csharp
public static class WorkspaceExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [RemoveCommentsAsync(Document, CommentKinds, CancellationToken)](RemoveCommentsAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_RemoveCommentsAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_CommentKinds_System_Threading_CancellationToken_) | Creates a new document with comments of the specified kind removed\. |
| [RemoveCommentsAsync(Document, TextSpan, CommentKinds, CancellationToken)](RemoveCommentsAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_RemoveCommentsAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_CommentKinds_System_Threading_CancellationToken_) | Creates a new document with comments of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, PreprocessorDirectiveKinds, CancellationToken)](RemovePreprocessorDirectivesAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_RemovePreprocessorDirectivesAsync_Microsoft_CodeAnalysis_Document_Roslynator_CSharp_PreprocessorDirectiveKinds_System_Threading_CancellationToken_) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemovePreprocessorDirectivesAsync(Document, TextSpan, PreprocessorDirectiveKinds, CancellationToken)](RemovePreprocessorDirectivesAsync/README.md#Roslynator_CSharp_WorkspaceExtensions_RemovePreprocessorDirectivesAsync_Microsoft_CodeAnalysis_Document_Microsoft_CodeAnalysis_Text_TextSpan_Roslynator_CSharp_PreprocessorDirectiveKinds_System_Threading_CancellationToken_) | Creates a new document with preprocessor directives of the specified kind removed\. |
| [RemoveRegionAsync(Document, RegionInfo, CancellationToken)](RemoveRegionAsync/README.md#_top) | Creates a new document with the specified region removed\. |
| [RemoveTriviaAsync(Document, TextSpan, CancellationToken)](RemoveTriviaAsync/README.md#_top) | Creates a new document with trivia inside the specified span removed\. |

