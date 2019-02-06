// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal sealed class CompilationModel
    {
        private readonly ImmutableArray<Compilation> _compilations;

        private CompilationModel(
            ImmutableArray<Compilation> compilations,
            Visibility visibility)
        {
            _compilations = compilations;
            Visibility = visibility;
        }

        public Visibility Visibility { get; }

        public IEnumerable<IAssemblySymbol> Assemblies
        {
            get { return _compilations.Select(kvp => kvp.Assembly); }
        }

        public IEnumerable<INamedTypeSymbol> Types
        {
            get { return Assemblies.SelectMany(assemblySymbol => assemblySymbol.GetTypes(typeSymbol => IsVisible(typeSymbol))); }
        }

        private bool IsVisible(ISymbol symbol)
        {
            return symbol.IsVisible(Visibility);
        }

        public static async Task<CompilationModel> CreateAsync(
            Solution solution,
            IEnumerable<string> projectNames = null,
            IEnumerable<string> ignoredProjectNames = null,
            string language = null,
            Visibility visibility = Visibility.Public,
            CancellationToken cancellationToken = default)
        {
            ImmutableArray<Compilation>.Builder compilations = ImmutableArray.CreateBuilder<Compilation>();

            foreach (Project project in FilterProjects())
            {
                Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
                compilations.Add(compilation);
            }

            return new CompilationModel(compilations.ToImmutableArray(), visibility);

            IEnumerable<Project> FilterProjects()
            {
                ImmutableHashSet<string> names = projectNames?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;

                ImmutableHashSet<string> ignoredNames = ignoredProjectNames?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;

                foreach (ProjectId projectId in solution
                    .GetProjectDependencyGraph()
                    .GetTopologicallySortedProjects(cancellationToken))
                {
                    Project project = solution.GetProject(projectId);

                    if ((language == null || language == project.Language)
                        && ((names.Count > 0) ? names.Contains(project.Name) : !ignoredNames.Contains(project.Name)))
                    {
                        yield return project;
                    }
                }
            }
        }

        public IEnumerable<IMethodSymbol> GetExtensionMethods()
        {
            foreach (INamedTypeSymbol typeSymbol in Types)
            {
                if (typeSymbol.MightContainExtensionMethods)
                {
                    foreach (ISymbol member in typeSymbol.GetMembers().Where(f => IsVisible(f)))
                    {
                        if (member.Kind == SymbolKind.Method
                            && member.IsStatic
                            && IsVisible(member))
                        {
                            var methodSymbol = (IMethodSymbol)member;

                            if (methodSymbol.IsExtensionMethod)
                                yield return methodSymbol;
                        }
                    }
                }
            }
        }

        public IEnumerable<IMethodSymbol> GetExtensionMethods(INamedTypeSymbol typeSymbol)
        {
            foreach (INamedTypeSymbol symbol in Types)
            {
                if (symbol.MightContainExtensionMethods)
                {
                    foreach (ISymbol member in symbol.GetMembers().Where(f => IsVisible(f)))
                    {
                        if (member.Kind == SymbolKind.Method
                            && member.IsStatic
                            && IsVisible(member))
                        {
                            var methodSymbol = (IMethodSymbol)member;

                            if (methodSymbol.IsExtensionMethod)
                            {
                                ITypeSymbol typeSymbol2 = GetExtendedType(methodSymbol);

                                if (typeSymbol == typeSymbol2)
                                    yield return methodSymbol;
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<INamedTypeSymbol> GetExtendedExternalTypes()
        {
            return Iterator().Distinct();

            IEnumerable<INamedTypeSymbol> Iterator()
            {
                foreach (IMethodSymbol methodSymbol in GetExtensionMethods())
                {
                    INamedTypeSymbol typeSymbol = GetExternalSymbol(methodSymbol);

                    if (typeSymbol != null)
                        yield return typeSymbol;
                }
            }

            INamedTypeSymbol GetExternalSymbol(IMethodSymbol methodSymbol)
            {
                INamedTypeSymbol type = GetExtendedType(methodSymbol);

                if (type == null)
                    return null;

                foreach (IAssemblySymbol assembly in Assemblies)
                {
                    if (type.ContainingAssembly == assembly)
                        return null;
                }

                return type;
            }
        }

        private static INamedTypeSymbol GetExtendedType(IMethodSymbol methodSymbol)
        {
            ITypeSymbol type = methodSymbol.Parameters[0].Type.OriginalDefinition;

            switch (type.Kind)
            {
                case SymbolKind.NamedType:
                    return (INamedTypeSymbol)type;
                case SymbolKind.TypeParameter:
                    return GetTypeParameterConstraintClass((ITypeParameterSymbol)type);
            }

            return null;

            INamedTypeSymbol GetTypeParameterConstraintClass(ITypeParameterSymbol typeParameter)
            {
                foreach (ITypeSymbol constraintType in typeParameter.ConstraintTypes)
                {
                    if (constraintType.TypeKind == TypeKind.Class)
                    {
                        return (INamedTypeSymbol)constraintType;
                    }
                    else if (constraintType.TypeKind == TypeKind.TypeParameter)
                    {
                        return GetTypeParameterConstraintClass((ITypeParameterSymbol)constraintType);
                    }
                }

                return null;
            }
        }

        public bool IsExternal(ISymbol symbol)
        {
            foreach (IAssemblySymbol assembly in Assemblies)
            {
                if (symbol.ContainingAssembly == assembly)
                    return false;
            }

            return true;
        }

        public IEnumerable<INamedTypeSymbol> GetDerivedTypes(INamedTypeSymbol symbol)
        {
            if (symbol.TypeKind.Is(TypeKind.Class, TypeKind.Interface)
                && !symbol.IsStatic)
            {
                foreach (INamedTypeSymbol typeSymbol in Types)
                {
                    if (typeSymbol.BaseType?.OriginalDefinition.Equals(symbol) == true)
                        yield return typeSymbol;

                    foreach (INamedTypeSymbol interfaceSymbol in typeSymbol.Interfaces)
                    {
                        if (interfaceSymbol.OriginalDefinition.Equals(symbol))
                            yield return typeSymbol;
                    }
                }
            }
        }

        public IEnumerable<INamedTypeSymbol> GetAllDerivedTypes(INamedTypeSymbol symbol)
        {
            if (symbol.TypeKind.Is(TypeKind.Class, TypeKind.Interface)
                && !symbol.IsStatic)
            {
                foreach (INamedTypeSymbol typeSymbol in Types)
                {
                    if (typeSymbol.InheritsFrom(symbol, includeInterfaces: true))
                        yield return typeSymbol;
                }
            }
        }
    }
}
