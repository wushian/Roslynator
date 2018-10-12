<a name="_top"></a>

# DiagnosticsExtensions\.ReportDiagnostic Method

[Home](../../../README.md#_top)

**Containing Type**: Roslynator\.[DiagnosticsExtensions](../README.md#_top)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, IEnumerable<Location> additionalLocations, ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**location**

**additionalLocations**

**properties**

**messageArgs**

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, IEnumerable<Location> additionalLocations, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**location**

**additionalLocations**

**messageArgs**

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**location**

**properties**

**messageArgs**

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location location, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

A [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor) describing the diagnostic\.

**location**

**messageArgs**

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode node, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**node**

**messageArgs**

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken token, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**token**

**messageArgs**

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this SymbolAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxTrivia trivia, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**trivia**

**messageArgs**

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, Location location, IEnumerable<Location> additionalLocations, ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**location**

**additionalLocations**

**properties**

**messageArgs**

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, Location location, IEnumerable<Location> additionalLocations, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**location**

**additionalLocations**

**messageArgs**

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**location**

**properties**

**messageArgs**

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, Location location, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**location**

**messageArgs**

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode node, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**node**

**messageArgs**

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken token, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**token**

**messageArgs**

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxTrivia trivia, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**trivia**

**messageArgs**

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this SyntaxTreeAnalysisContext context, DiagnosticDescriptor descriptor, Location location, IEnumerable<Location> additionalLocations, ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**location**

**additionalLocations**

**properties**

**messageArgs**

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this SyntaxTreeAnalysisContext context, DiagnosticDescriptor descriptor, Location location, IEnumerable<Location> additionalLocations, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**location**

**additionalLocations**

**messageArgs**

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this SyntaxTreeAnalysisContext context, DiagnosticDescriptor descriptor, Location location, ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**location**

**properties**

**messageArgs**

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this SyntaxTreeAnalysisContext context, DiagnosticDescriptor descriptor, Location location, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**location**

**messageArgs**

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this SyntaxTreeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxNode node, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**node**

**messageArgs**

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this SyntaxTreeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxToken token, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**token**

**messageArgs**

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\]\) <a name="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___"></a>

### Summary

Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this SyntaxTreeAnalysisContext context, DiagnosticDescriptor descriptor, SyntaxTrivia trivia, params object[] messageArgs)
```

### Parameters

**context**

**descriptor**

**trivia**

**messageArgs**