﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about modifier list.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct ModifierListInfo
    {
        internal ModifierListInfo(SyntaxNode parent, SyntaxTokenList modifiers)
        {
            Parent = parent;
            Modifiers = modifiers;
        }

        /// <summary>
        /// The node that contains the modifiers.
        /// </summary>
        public SyntaxNode Parent { get; }

        /// <summary>
        /// The modifier list.
        /// </summary>
        public SyntaxTokenList Modifiers { get; }

        /// <summary>
        /// The explicit accessibility.
        /// </summary>
        public Accessibility ExplicitAccessibility
        {
            get { return SyntaxAccessibility.GetExplicitAccessibility(Modifiers); }
        }

        /// <summary>
        /// True if the modifier list contains "new" modifier.
        /// </summary>
        public bool IsNew => Modifiers.Contains(SyntaxKind.NewKeyword);

        /// <summary>
        /// True if the modifier list contains "const" modifier.
        /// </summary>
        public bool IsConst => Modifiers.Contains(SyntaxKind.ConstKeyword);

        /// <summary>
        /// True if the modifier list contains "static" modifier.
        /// </summary>
        public bool IsStatic => Modifiers.Contains(SyntaxKind.StaticKeyword);

        /// <summary>
        /// True if the modifier list contains "virtual" modifier.
        /// </summary>
        public bool IsVirtual => Modifiers.Contains(SyntaxKind.VirtualKeyword);

        /// <summary>
        /// True if the modifier list contains "sealed" modifier.
        /// </summary>
        public bool IsSealed => Modifiers.Contains(SyntaxKind.SealedKeyword);

        /// <summary>
        /// True if the modifier list contains "override" modifier.
        /// </summary>
        public bool IsOverride => Modifiers.Contains(SyntaxKind.OverrideKeyword);

        /// <summary>
        /// True if the modifier list contains "abstract" modifier.
        /// </summary>
        public bool IsAbstract => Modifiers.Contains(SyntaxKind.AbstractKeyword);

        /// <summary>
        /// True if the modifier list contains "readonly" modifier.
        /// </summary>
        public bool IsReadOnly => Modifiers.Contains(SyntaxKind.ReadOnlyKeyword);

        /// <summary>
        /// True if the modifier list contains "extern" modifier.
        /// </summary>
        public bool IsExtern => Modifiers.Contains(SyntaxKind.ExternKeyword);

        /// <summary>
        /// True if the modifier list contains "unsafe" modifier.
        /// </summary>
        public bool IsUnsafe => Modifiers.Contains(SyntaxKind.UnsafeKeyword);

        /// <summary>
        /// True if the modifier list contains "volatile" modifier.
        /// </summary>
        public bool IsVolatile => Modifiers.Contains(SyntaxKind.VolatileKeyword);

        /// <summary>
        /// True if the modifier list contains "async" modifier.
        /// </summary>
        public bool IsAsync => Modifiers.Contains(SyntaxKind.AsyncKeyword);

        /// <summary>
        /// True if the modifier list contains "partial" modifier.
        /// </summary>
        public bool IsPartial => Modifiers.Contains(SyntaxKind.PartialKeyword);

        /// <summary>
        /// True if the modifier list contains "ref" modifier.
        /// </summary>
        public bool IsRef => Modifiers.Contains(SyntaxKind.RefKeyword);

        /// <summary>
        /// True if the modifier list contains "out" modifier.
        /// </summary>
        public bool IsOut => Modifiers.Contains(SyntaxKind.OutKeyword);

        /// <summary>
        /// True if the modifier list contains "in" modifier.
        /// </summary>
        public bool IsIn => Modifiers.Contains(SyntaxKind.InKeyword);

        /// <summary>
        /// True if the modifier list contains "params" modifier.
        /// </summary>
        public bool IsParams => Modifiers.Contains(SyntaxKind.ParamsKeyword);

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return Parent != null; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return (Success)
                     ? $"{GetType().Name} {Modifiers} {Parent.Kind()}"
                     : "Uninitialized";
            }
        }

        internal static ModifierListInfo Create(SyntaxNode node)
        {
            if (node == null)
                return default;

            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return new ModifierListInfo(node, ((ClassDeclarationSyntax)node).Modifiers);
                case SyntaxKind.ConstructorDeclaration:
                    return new ModifierListInfo(node, ((ConstructorDeclarationSyntax)node).Modifiers);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return new ModifierListInfo(node, ((ConversionOperatorDeclarationSyntax)node).Modifiers);
                case SyntaxKind.DelegateDeclaration:
                    return new ModifierListInfo(node, ((DelegateDeclarationSyntax)node).Modifiers);
                case SyntaxKind.DestructorDeclaration:
                    return new ModifierListInfo(node, ((DestructorDeclarationSyntax)node).Modifiers);
                case SyntaxKind.EnumDeclaration:
                    return new ModifierListInfo(node, ((EnumDeclarationSyntax)node).Modifiers);
                case SyntaxKind.EventDeclaration:
                    return new ModifierListInfo(node, ((EventDeclarationSyntax)node).Modifiers);
                case SyntaxKind.EventFieldDeclaration:
                    return new ModifierListInfo(node, ((EventFieldDeclarationSyntax)node).Modifiers);
                case SyntaxKind.FieldDeclaration:
                    return new ModifierListInfo(node, ((FieldDeclarationSyntax)node).Modifiers);
                case SyntaxKind.IndexerDeclaration:
                    return new ModifierListInfo(node, ((IndexerDeclarationSyntax)node).Modifiers);
                case SyntaxKind.InterfaceDeclaration:
                    return new ModifierListInfo(node, ((InterfaceDeclarationSyntax)node).Modifiers);
                case SyntaxKind.MethodDeclaration:
                    return new ModifierListInfo(node, ((MethodDeclarationSyntax)node).Modifiers);
                case SyntaxKind.OperatorDeclaration:
                    return new ModifierListInfo(node, ((OperatorDeclarationSyntax)node).Modifiers);
                case SyntaxKind.PropertyDeclaration:
                    return new ModifierListInfo(node, ((PropertyDeclarationSyntax)node).Modifiers);
                case SyntaxKind.StructDeclaration:
                    return new ModifierListInfo(node, ((StructDeclarationSyntax)node).Modifiers);
                case SyntaxKind.IncompleteMember:
                    return new ModifierListInfo(node, ((IncompleteMemberSyntax)node).Modifiers);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return new ModifierListInfo(node, ((AccessorDeclarationSyntax)node).Modifiers);
                case SyntaxKind.LocalDeclarationStatement:
                    return new ModifierListInfo(node, ((LocalDeclarationStatementSyntax)node).Modifiers);
                case SyntaxKind.LocalFunctionStatement:
                    return new ModifierListInfo(node, ((LocalFunctionStatementSyntax)node).Modifiers);
                case SyntaxKind.Parameter:
                    return new ModifierListInfo(node, ((ParameterSyntax)node).Modifiers);
            }

            return default;
        }

        internal static ModifierListInfo Create(ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration == null)
                return default;

            return new ModifierListInfo(classDeclaration, classDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                return default;

            return new ModifierListInfo(constructorDeclaration, constructorDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
        {
            if (conversionOperatorDeclaration == null)
                return default;

            return new ModifierListInfo(conversionOperatorDeclaration, conversionOperatorDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(DelegateDeclarationSyntax delegateDeclaration)
        {
            if (delegateDeclaration == null)
                return default;

            return new ModifierListInfo(delegateDeclaration, delegateDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(DestructorDeclarationSyntax destructorDeclaration)
        {
            if (destructorDeclaration == null)
                return default;

            return new ModifierListInfo(destructorDeclaration, destructorDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(EnumDeclarationSyntax enumDeclaration)
        {
            if (enumDeclaration == null)
                return default;

            return new ModifierListInfo(enumDeclaration, enumDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(EventDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration == null)
                return default;

            return new ModifierListInfo(eventDeclaration, eventDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (eventFieldDeclaration == null)
                return default;

            return new ModifierListInfo(eventFieldDeclaration, eventFieldDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration == null)
                return default;

            return new ModifierListInfo(fieldDeclaration, fieldDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                return default;

            return new ModifierListInfo(indexerDeclaration, indexerDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (interfaceDeclaration == null)
                return default;

            return new ModifierListInfo(interfaceDeclaration, interfaceDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                return default;

            return new ModifierListInfo(methodDeclaration, methodDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(OperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration == null)
                return default;

            return new ModifierListInfo(operatorDeclaration, operatorDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                return default;

            return new ModifierListInfo(propertyDeclaration, propertyDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(StructDeclarationSyntax structDeclaration)
        {
            if (structDeclaration == null)
                return default;

            return new ModifierListInfo(structDeclaration, structDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(IncompleteMemberSyntax incompleteMember)
        {
            if (incompleteMember == null)
                return default;

            return new ModifierListInfo(incompleteMember, incompleteMember.Modifiers);
        }

        internal static ModifierListInfo Create(AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                return default;

            return new ModifierListInfo(accessorDeclaration, accessorDeclaration.Modifiers);
        }

        internal static ModifierListInfo Create(LocalDeclarationStatementSyntax localDeclarationStatement)
        {
            if (localDeclarationStatement == null)
                return default;

            return new ModifierListInfo(localDeclarationStatement, localDeclarationStatement.Modifiers);
        }

        internal static ModifierListInfo Create(LocalFunctionStatementSyntax localFunctionStatement)
        {
            if (localFunctionStatement == null)
                return default;

            return new ModifierListInfo(localFunctionStatement, localFunctionStatement.Modifiers);
        }

        internal static ModifierListInfo Create(ParameterSyntax parameter)
        {
            if (parameter == null)
                return default;

            return new ModifierListInfo(parameter, parameter.Modifiers);
        }

        /// <summary>
        /// Creates a new <see cref="ModifierListInfo"/> with accessibility modifiers removed.
        /// </summary>
        public ModifierListInfo WithoutExplicitAccessibility()
        {
            return WithExplicitAccessibility(Accessibility.NotApplicable);
        }

        /// <summary>
        /// Creates a new <see cref="ModifierListInfo"/> with accessibility modifiers updated.
        /// </summary>
        /// <param name="newAccessibility"></param>
        /// <param name="comparer"></param>
        public ModifierListInfo WithExplicitAccessibility(Accessibility newAccessibility, IComparer<SyntaxKind> comparer = null)
        {
            ThrowInvalidOperationIfNotInitialized();

            Accessibility accessibility = ExplicitAccessibility;

            if (accessibility == newAccessibility)
                return this;

            comparer = comparer ?? ModifierKindComparer.Default;

            SyntaxNode declaration = Parent;

            if (accessibility.IsSingleTokenAccessibility()
                && newAccessibility.IsSingleTokenAccessibility())
            {
                int insertIndex = ModifierList.GetInsertIndex(Modifiers, GetTokenKind(), comparer);

                int tokenIndex = GetFirstTokenIndex();

                if (tokenIndex == insertIndex
                    || tokenIndex == insertIndex - 1)
                {
                    SyntaxToken token = Modifiers[tokenIndex];

                    SyntaxToken newToken = SyntaxFactory.Token(GetTokenKind()).WithTriviaFrom(token);

                    SyntaxTokenList newModifiers = Modifiers.Replace(token, newToken);

                    return WithModifiers(newModifiers);
                }
            }

            if (accessibility != Accessibility.NotApplicable)
            {
                (int tokenIndex, int secondTokenIndex) = GetTokenIndexes();

                declaration = ModifierList.RemoveAt(declaration, Math.Max(tokenIndex, secondTokenIndex));

                if (secondTokenIndex != -1)
                    declaration = ModifierList.RemoveAt(declaration, Math.Min(tokenIndex, secondTokenIndex));
            }

            if (newAccessibility != Accessibility.NotApplicable)
                declaration = ModifierList.Insert(declaration, newAccessibility, comparer);

            return SyntaxInfo.ModifierListInfo(declaration);

            SyntaxKind GetTokenKind()
            {
                switch (newAccessibility)
                {
                    case Accessibility.Private:
                        return SyntaxKind.PrivateKeyword;
                    case Accessibility.Protected:
                        return SyntaxKind.ProtectedKeyword;
                    case Accessibility.Internal:
                        return SyntaxKind.InternalKeyword;
                    case Accessibility.Public:
                        return SyntaxKind.PublicKeyword;
                    case Accessibility.NotApplicable:
                        return SyntaxKind.None;
                    default:
                        throw new ArgumentException("", nameof(newAccessibility));
                }
            }
        }

        private (int firstIndex, int secondIndex) GetTokenIndexes()
        {
            int count = Modifiers.Count;

            for (int i = 0; i < count; i++)
            {
                switch (Modifiers[i].Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        {
                            return (i, -1);
                        }
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.InternalKeyword:
                        {
                            for (int j = i + 1; j < count; j++)
                            {
                                if (Modifiers[j].IsKind(SyntaxKind.ProtectedKeyword))
                                    return (i, j);
                            }

                            return (i, -1);
                        }
                    case SyntaxKind.ProtectedKeyword:
                        {
                            for (int j = i + 1; j < count; j++)
                            {
                                if (Modifiers[j].IsKind(SyntaxKind.InternalKeyword, SyntaxKind.PrivateKeyword))
                                    return (i, j);
                            }

                            return (i, -1);
                        }
                }
            }

            return (-1, -1);
        }

        private int GetFirstTokenIndex()
        {
            int count = Modifiers.Count;

            for (int i = 0; i < count; i++)
            {
                switch (Modifiers[i].Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                        {
                            return i;
                        }
                }
            }

            return -1;
        }

        /// <summary>
        /// Creates a new <see cref="ModifierListInfo"/> with the specified modifiers updated.
        /// </summary>
        /// <param name="modifiers"></param>
        public ModifierListInfo WithModifiers(SyntaxTokenList modifiers)
        {
            ThrowInvalidOperationIfNotInitialized();

            switch (Parent.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)Parent;
                        ClassDeclarationSyntax newNode = classDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)Parent;
                        ConstructorDeclarationSyntax newNode = constructorDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)Parent;
                        OperatorDeclarationSyntax newNode = operatorDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)Parent;
                        ConversionOperatorDeclarationSyntax newNode = conversionOperatorDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)Parent;
                        DelegateDeclarationSyntax newNode = delegateDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        var destructorDeclaration = (DestructorDeclarationSyntax)Parent;
                        DestructorDeclarationSyntax newNode = destructorDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.EnumDeclaration:
                    {
                        var enumDeclaration = (EnumDeclarationSyntax)Parent;
                        EnumDeclarationSyntax newNode = enumDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)Parent;
                        EventDeclarationSyntax newNode = eventDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        var eventFieldDeclaration = (EventFieldDeclarationSyntax)Parent;
                        EventFieldDeclarationSyntax newNode = eventFieldDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.FieldDeclaration:
                    {
                        var fieldDeclaration = (FieldDeclarationSyntax)Parent;
                        FieldDeclarationSyntax newNode = fieldDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)Parent;
                        IndexerDeclarationSyntax newNode = indexerDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)Parent;
                        InterfaceDeclarationSyntax newNode = interfaceDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)Parent;
                        MethodDeclarationSyntax newNode = methodDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)Parent;
                        PropertyDeclarationSyntax newNode = propertyDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)Parent;
                        StructDeclarationSyntax newNode = structDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.IncompleteMember:
                    {
                        var incompleteMember = (IncompleteMemberSyntax)Parent;
                        IncompleteMemberSyntax newNode = incompleteMember.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    {
                        var accessorDeclaration = (AccessorDeclarationSyntax)Parent;
                        AccessorDeclarationSyntax newNode = accessorDeclaration.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        var localDeclarationStatement = (LocalDeclarationStatementSyntax)Parent;
                        LocalDeclarationStatementSyntax newNode = localDeclarationStatement.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunctionStatement = (LocalFunctionStatementSyntax)Parent;
                        LocalFunctionStatementSyntax newNode = localFunctionStatement.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
                case SyntaxKind.Parameter:
                    {
                        var parameter = (ParameterSyntax)Parent;
                        ParameterSyntax newNode = parameter.WithModifiers(modifiers);
                        return new ModifierListInfo(newNode, newNode.Modifiers);
                    }
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the modifier filter.
        /// </summary>
        public ModifierFilter GetFilter()
        {
            var filter = ModifierFilter.None;

            for (int i = 0; i < Modifiers.Count; i++)
            {
                switch (Modifiers[i].Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        {
                            filter |= ModifierFilter.Public;
                            break;
                        }
                    case SyntaxKind.PrivateKeyword:
                        {
                            filter |= ModifierFilter.Private;
                            break;
                        }
                    case SyntaxKind.InternalKeyword:
                        {
                            filter |= ModifierFilter.Internal;
                            break;
                        }
                    case SyntaxKind.ProtectedKeyword:
                        {
                            filter |= ModifierFilter.Protected;
                            break;
                        }
                    case SyntaxKind.StaticKeyword:
                        {
                            filter |= ModifierFilter.Static;
                            break;
                        }
                    case SyntaxKind.ReadOnlyKeyword:
                        {
                            filter |= ModifierFilter.ReadOnly;
                            break;
                        }
                    case SyntaxKind.SealedKeyword:
                        {
                            filter |= ModifierFilter.Sealed;
                            break;
                        }
                    case SyntaxKind.ConstKeyword:
                        {
                            filter |= ModifierFilter.Const;
                            break;
                        }
                    case SyntaxKind.VolatileKeyword:
                        {
                            filter |= ModifierFilter.Volatile;
                            break;
                        }
                    case SyntaxKind.NewKeyword:
                        {
                            filter |= ModifierFilter.New;
                            break;
                        }
                    case SyntaxKind.OverrideKeyword:
                        {
                            filter |= ModifierFilter.Override;
                            break;
                        }
                    case SyntaxKind.AbstractKeyword:
                        {
                            filter |= ModifierFilter.Abstract;
                            break;
                        }
                    case SyntaxKind.VirtualKeyword:
                        {
                            filter |= ModifierFilter.Virtual;
                            break;
                        }
                    case SyntaxKind.RefKeyword:
                        {
                            filter |= ModifierFilter.Ref;
                            break;
                        }
                    case SyntaxKind.OutKeyword:
                        {
                            filter |= ModifierFilter.Out;
                            break;
                        }
                    case SyntaxKind.InKeyword:
                        {
                            filter |= ModifierFilter.In;
                            break;
                        }
                    case SyntaxKind.ParamsKeyword:
                        {
                            filter |= ModifierFilter.Params;
                            break;
                        }
                    case SyntaxKind.UnsafeKeyword:
                        {
                            filter |= ModifierFilter.Unsafe;
                            break;
                        }
                    case SyntaxKind.PartialKeyword:
                        {
                            filter |= ModifierFilter.Partial;
                            break;
                        }
                    case SyntaxKind.AsyncKeyword:
                        {
                            filter |= ModifierFilter.Async;
                            break;
                        }
                    case SyntaxKind.ExternKeyword:
                        {
                            filter |= ModifierFilter.Extern;
                            break;
                        }
                    default:
                        {
                            Debug.Fail(Modifiers[i].Kind().ToString());
                            break;
                        }
                }
            }

            return filter;
        }

        private void ThrowInvalidOperationIfNotInitialized()
        {
            if (Parent == null)
                throw new InvalidOperationException($"{nameof(ModifierListInfo)} is not initalized.");
        }
    }
}
