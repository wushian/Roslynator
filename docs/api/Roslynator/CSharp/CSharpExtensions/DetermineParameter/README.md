<a name="_top"></a>

# CSharpExtensions\.DetermineParameter Method

[Home](../../../../README.md#_top)

**Containing Type**: Roslynator\.CSharp\.[CSharpExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [DetermineParameter(SemanticModel, ArgumentSyntax, Boolean, Boolean, CancellationToken)](#Roslynator_CSharp_CSharpExtensions_DetermineParameter_Microsoft_CodeAnalysis_SemanticModel_Microsoft_CodeAnalysis_CSharp_Syntax_ArgumentSyntax_System_Boolean_System_Boolean_System_Threading_CancellationToken_) | Determines a parameter symbol that matches to the specified argument\. Returns null if no matching parameter is found\. |
| [DetermineParameter(SemanticModel, AttributeArgumentSyntax, Boolean, Boolean, CancellationToken)](#Roslynator_CSharp_CSharpExtensions_DetermineParameter_Microsoft_CodeAnalysis_SemanticModel_Microsoft_CodeAnalysis_CSharp_Syntax_AttributeArgumentSyntax_System_Boolean_System_Boolean_System_Threading_CancellationToken_) | Determines a parameter symbol that matches to the specified attribute argument\. Returns null if not matching parameter is found\. |

## DetermineParameter\(SemanticModel, ArgumentSyntax, Boolean, Boolean, CancellationToken\) <a name="Roslynator_CSharp_CSharpExtensions_DetermineParameter_Microsoft_CodeAnalysis_SemanticModel_Microsoft_CodeAnalysis_CSharp_Syntax_ArgumentSyntax_System_Boolean_System_Boolean_System_Threading_CancellationToken_"></a>

### Summary

Determines a parameter symbol that matches to the specified argument\.
Returns null if no matching parameter is found\.

```csharp
public static IParameterSymbol DetermineParameter(this SemanticModel semanticModel, ArgumentSyntax argument, bool allowParams = false, bool allowCandidate = false, CancellationToken cancellationToken = default(CancellationToken))
```

### Parameters

**semanticModel**

**argument**

**allowParams**

**allowCandidate**

**cancellationToken**

### Returns

Microsoft\.CodeAnalysis\.[IParameterSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.iparametersymbol)

## DetermineParameter\(SemanticModel, AttributeArgumentSyntax, Boolean, Boolean, CancellationToken\) <a name="Roslynator_CSharp_CSharpExtensions_DetermineParameter_Microsoft_CodeAnalysis_SemanticModel_Microsoft_CodeAnalysis_CSharp_Syntax_AttributeArgumentSyntax_System_Boolean_System_Boolean_System_Threading_CancellationToken_"></a>

### Summary

Determines a parameter symbol that matches to the specified attribute argument\.
Returns null if not matching parameter is found\.

```csharp
public static IParameterSymbol DetermineParameter(this SemanticModel semanticModel, AttributeArgumentSyntax attributeArgument, bool allowParams = false, bool allowCandidate = false, CancellationToken cancellationToken = default(CancellationToken))
```

### Parameters

**semanticModel**

**attributeArgument**

**allowParams**

**allowCandidate**

**cancellationToken**

### Returns

Microsoft\.CodeAnalysis\.[IParameterSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.iparametersymbol)

