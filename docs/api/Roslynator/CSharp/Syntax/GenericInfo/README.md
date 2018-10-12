# GenericInfo Struct

[Home](../../../../README.md) &#x2022; [Properties](#properties) &#x2022; [Methods](#methods) &#x2022; [Operators](#operators)

**Namespace**: [Roslynator.CSharp.Syntax](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Summary

Provides information about generic syntax \(class, struct, interface, delegate, method or local function\)\.

```csharp
public readonly struct GenericInfo : System.IEquatable<GenericInfo>
```

### Inheritance

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) &#x2192; [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) &#x2192; GenericInfo

### Implements

* System\.[IEquatable](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)\<[GenericInfo](./README.md)>

## Properties

| Property | Summary |
| -------- | ------- |
| [ConstraintClauses](ConstraintClauses/README.md) | A list of constraint clauses\. |
| [Kind](Kind/README.md) | The kind of this syntax node\. |
| [Node](Node/README.md) | The syntax node that can be generic \(for example [ClassDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.classdeclarationsyntax) for a class or [LocalDeclarationStatementSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.localdeclarationstatementsyntax) for a local function\)\. |
| [Success](Success/README.md) | Determines whether this struct was initialized with an actual syntax\. |
| [TypeParameterList](TypeParameterList/README.md) | The type parameter list\. |
| [TypeParameters](TypeParameters/README.md) | A list of type parameters\. |

## Methods

| Method | Summary |
| ------ | ------- |
| [Equals(GenericInfo)](Equals/README.md#Roslynator_CSharp_Syntax_GenericInfo_Equals_Roslynator_CSharp_Syntax_GenericInfo_) | Determines whether this instance is equal to another object of the same type\. \(Implements [IEquatable\<GenericInfo>.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1.equals)\) |
| [Equals(Object)](Equals/README.md#Roslynator_CSharp_Syntax_GenericInfo_Equals_System_Object_) | Determines whether this instance and a specified object are equal\. \(Overrides [ValueType.Equals](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.equals)\) |
| [FindConstraintClause(String)](FindConstraintClause/README.md) | Searches for a constraint clause with the specified type parameter name and returns the first occurrence within the constraint clauses\. |
| [FindTypeParameter(String)](FindTypeParameter/README.md) | Searches for a type parameter with the specified name and returns the first occurrence within the type parameters\. |
| [GetHashCode()](GetHashCode/README.md) | Returns the hash code for this instance\. \(Overrides [ValueType.GetHashCode](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.gethashcode)\) |
| [GetType()](https://docs.microsoft.com/en-us/dotnet/api/system.object.gettype) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [MemberwiseClone()](https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone) |  \(Inherited from [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\) |
| [RemoveAllConstraintClauses()](RemoveAllConstraintClauses/README.md) | Creates a new [GenericInfo](./README.md) with all constraint clauses removed\. |
| [RemoveConstraintClause(TypeParameterConstraintClauseSyntax)](RemoveConstraintClause/README.md) | Creates a new [GenericInfo](./README.md) with the specified constraint clause removed\. |
| [RemoveTypeParameter(TypeParameterSyntax)](RemoveTypeParameter/README.md) | Creates a new [GenericInfo](./README.md) with the specified type parameter removed\. |
| [ToString()](ToString/README.md) | Returns the string representation of the underlying syntax, not including its leading and trailing trivia\. \(Overrides [ValueType.ToString](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype.tostring)\) |
| [WithConstraintClauses(SyntaxList\<TypeParameterConstraintClauseSyntax>)](WithConstraintClauses/README.md) | Creates a new [GenericInfo](./README.md) with the constraint clauses updated\. |
| [WithTypeParameterList(TypeParameterListSyntax)](WithTypeParameterList/README.md) | Creates a new [GenericInfo](./README.md) with the type parameter list updated\. |

## Operators

| Operator | Summary |
| -------- | ------- |
| [Equality(GenericInfo, GenericInfo)](op_Equality/README.md) | |
| [Inequality(GenericInfo, GenericInfo)](op_Inequality/README.md) | |

