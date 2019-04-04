// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertInterfaceToAbstractClassRefactoring
    {
        internal static void ComputeRefactorings(RefactoringContext context, InterfaceDeclarationSyntax interfaceDeclaration)
        {
            SyntaxList<MemberDeclarationSyntax> members = interfaceDeclaration.Members;

            if (!members.Any())
                return;

            Document document = context.Document;

            context.RegisterRefactoring(
                $"Convert '{interfaceDeclaration.Identifier.ValueText}' to abstract class",
                ct => RefactorAsync(document, interfaceDeclaration, ct),
                RefactoringIdentifiers.ConvertInterfaceToAbstractClass);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            InterfaceDeclarationSyntax interfaceDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxList<MemberDeclarationSyntax> members = interfaceDeclaration.Members.Select<MemberDeclarationSyntax, MemberDeclarationSyntax> (member =>
            {
                switch (member)
                {
                    case MethodDeclarationSyntax methodDeclaration:
                        {
                            return methodDeclaration.Update(
                                attributeLists: methodDeclaration.AttributeLists,
                                modifiers: Modifiers.Public_Abstract(),
                                returnType: methodDeclaration.ReturnType,
                                explicitInterfaceSpecifier: methodDeclaration.ExplicitInterfaceSpecifier,
                                identifier: methodDeclaration.Identifier,
                                typeParameterList: methodDeclaration.TypeParameterList,
                                parameterList: methodDeclaration.ParameterList,
                                constraintClauses: methodDeclaration.ConstraintClauses,
                                body: default(BlockSyntax),
                                expressionBody: default(ArrowExpressionClauseSyntax),
                                semicolonToken: methodDeclaration.SemicolonToken);
                        }
                    case PropertyDeclarationSyntax propertyDeclaration:
                        {
                            return propertyDeclaration.Update(
                                attributeLists: propertyDeclaration.AttributeLists,
                                modifiers: Modifiers.Public_Abstract(),
                                type: propertyDeclaration.Type,
                                explicitInterfaceSpecifier: propertyDeclaration.ExplicitInterfaceSpecifier,
                                identifier: propertyDeclaration.Identifier,
                                accessorList: propertyDeclaration.AccessorList,
                                expressionBody: default(ArrowExpressionClauseSyntax),
                                initializer: default(EqualsValueClauseSyntax),
                                semicolonToken: propertyDeclaration.SemicolonToken);
                        }
                    case EventFieldDeclarationSyntax eventFieldDeclaration:
                        {
                            return eventFieldDeclaration.Update(
                                attributeLists: eventFieldDeclaration.AttributeLists,
                                modifiers: Modifiers.Public_Abstract(),
                                eventKeyword: eventFieldDeclaration.EventKeyword,
                                declaration: eventFieldDeclaration.Declaration,
                                semicolonToken: eventFieldDeclaration.SemicolonToken);
                        }
                    case IndexerDeclarationSyntax indexerDeclaration:
                        {
                            return indexerDeclaration.Update(
                                attributeLists: indexerDeclaration.AttributeLists,
                                modifiers: Modifiers.Public_Abstract(),
                                type: indexerDeclaration.Type,
                                explicitInterfaceSpecifier: indexerDeclaration.ExplicitInterfaceSpecifier,
                                thisKeyword: indexerDeclaration.ThisKeyword,
                                parameterList: indexerDeclaration.ParameterList,
                                accessorList: indexerDeclaration.AccessorList,
                                expressionBody: default(ArrowExpressionClauseSyntax),
                                semicolonToken: indexerDeclaration.SemicolonToken);
                        }
                    default:
                        {
                            Debug.Fail(member.Kind().ToString());
                            return null;
                        }
                }
            })
            .Where(member => member != null)
            .ToSyntaxList();

            SyntaxToken identifier = interfaceDeclaration.Identifier;

            string name = identifier.ValueText;

            if (name.StartsWith("I"))
                name = name.Substring(1);

            SyntaxToken classIdentifier = Identifier(identifier.LeadingTrivia, name, identifier.TrailingTrivia);

            ClassDeclarationSyntax classDeclaration = ClassDeclaration(
                interfaceDeclaration.Modifiers.AddRange(TokenList(Token(SyntaxKind.AbstractKeyword))),
                classIdentifier.WithNavigationAnnotation(),
                members);

            classDeclaration = classDeclaration.WithFormatterAnnotation();

            MemberDeclarationListInfo membersInfo = SyntaxInfo.MemberDeclarationListInfo(interfaceDeclaration.Parent);

            SyntaxList<MemberDeclarationSyntax> newMembers = membersInfo.Members.Insert(membersInfo.IndexOf(interfaceDeclaration) + 1, classDeclaration);

            return document.ReplaceMembersAsync(membersInfo, newMembers, cancellationToken);
        }
    }
}