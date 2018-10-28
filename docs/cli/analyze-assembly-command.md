
# `analyze-assembly` Command

Searches file or directory for analyzer assemblies.

## Synopsis

```
roslynator analyze-assembly <PATH>
[--language]
[--log-file]
[--log-file-verbosity]
[--no-analyzers]
[--no-fixers]
[-v|--verbosity]
```

## Arguments

**`PATH`**

The path to file or directory to analyze.

### Optional Options

**`--language`** `{csharp|vb}`

Defines project language.

**`--no-analyzers`**

Indicates whether to search for DiagnosticAnalyzers.

**`--no-fixers`**

Indicates whether to search for CodeFixProviders.

**`-v|--verbosity`** `{q[uiet]|m[inimal]|n[ormal]|d[etailed]}`

Defines the amount of information to display in the log.

## See Also

* [Roslynator Command-Line Interface](README.md)
