# StatementListSelection\.Create Method

[Home](../../../../README.md)

**Containing Type**: Roslynator\.CSharp\.[StatementListSelection](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Create(BlockSyntax, TextSpan)](#Roslynator_CSharp_StatementListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_BlockSyntax_Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [StatementListSelection](../README.md) based on the specified block and span\. |
| [Create(StatementListInfo, TextSpan)](#Roslynator_CSharp_StatementListSelection_Create_Roslynator_CSharp_Syntax_StatementListInfo__Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [StatementListSelection](../README.md) based on the specified [StatementListInfo](../../Syntax/StatementListInfo/README.md) and span\. |
| [Create(SwitchSectionSyntax, TextSpan)](#Roslynator_CSharp_StatementListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_SwitchSectionSyntax_Microsoft_CodeAnalysis_Text_TextSpan_) | Creates a new [StatementListSelection](../README.md) based on the specified switch section and span\. |

## Create\(BlockSyntax, TextSpan\) <a name="Roslynator_CSharp_StatementListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_BlockSyntax_Microsoft_CodeAnalysis_Text_TextSpan_"></a>

### Summary

Creates a new [StatementListSelection](../README.md) based on the specified block and span\.

```csharp
public static StatementListSelection Create(BlockSyntax block, TextSpan span)
```

### Parameters

**block**

**span**

### Returns

Roslynator\.CSharp\.[StatementListSelection](../README.md)

## Create\(StatementListInfo, TextSpan\) <a name="Roslynator_CSharp_StatementListSelection_Create_Roslynator_CSharp_Syntax_StatementListInfo__Microsoft_CodeAnalysis_Text_TextSpan_"></a>

### Summary

Creates a new [StatementListSelection](../README.md) based on the specified [StatementListInfo](../../Syntax/StatementListInfo/README.md) and span\.

```csharp
public static StatementListSelection Create(in StatementListInfo statementsInfo, TextSpan span)
```

### Parameters

**statementsInfo**

**span**

### Returns

Roslynator\.CSharp\.[StatementListSelection](../README.md)

## Create\(SwitchSectionSyntax, TextSpan\) <a name="Roslynator_CSharp_StatementListSelection_Create_Microsoft_CodeAnalysis_CSharp_Syntax_SwitchSectionSyntax_Microsoft_CodeAnalysis_Text_TextSpan_"></a>

### Summary

Creates a new [StatementListSelection](../README.md) based on the specified switch section and span\.

```csharp
public static StatementListSelection Create(SwitchSectionSyntax switchSection, TextSpan span)
```

### Parameters

**switchSection**

**span**

### Returns

Roslynator\.CSharp\.[StatementListSelection](../README.md)

