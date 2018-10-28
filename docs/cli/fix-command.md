
# `fix` Command

Fixes all diagnostics in the specified project or solution.

## Synopsis

```
roslynator fix <PROJECT|SOLUTION>
[-a|--analyzer-assemblies]
[--batch-size]
[--culture]
[--format]
[--ignore-analyzer-references]
[--ignore-compiler-errors]
[--ignored-compiler-diagnostics]
[--ignored-diagnostics]
[--ignored-projects]
[--language]
[--file-log]
[--file-log-verbosity]
[--minimal-severity]
[--msbuild-path]
[--projects]
[-p|--properties]
[--supported-diagnostics]
[--use-roslynator-analyzers]
[-v|--verbosity]
```

## Arguments

**`PROJECT|SOLUTION`**

The project or solution to fix.

### Optional Options

**`a-|--analyzer-assemblies`**

Defines one or more paths to:

* analyzer assembly
* directory that should be searched recursively for analyzer assemblies

**`--batch-size`**

Defines maximum number of diagnostics that can be fixed in one batch.

**`--culture`**

Defines culture that should be used to display diagnostic message.

**`--format`**

Indicates whether each document should be formatted.

**`--ignore-analyzer-references`**

Indicates whether Roslynator should ignore analyzers that are referenced in projects.

**`--ignore-compiler-errors`**

Indicates whether fixing should continue even if compilation has errors.

**`--ignored-compiler-diagnostics`**

Defines compiler diagnostic identifiers that should be ignored even if `--ignore-compiler-errors` is not set.

**`--ignored-diagnostics`**

Defines diagnostic identifiers that should not be fixed.

**`--ignored-projects`**

Defines projects that should not be fixed.

**`--language`** `{cs[harp]|v[isual-]b[asic])}`

Defines project language.

**`--minimal-severity`** `{hidden|info|warning|error}`

Defines minimal severity for a diagnostic. Default value is `info`.

**`--msbuild-path`**

Defines a path to MSBuild.

*Note: If the path to MSBuild is not specified and there are installed multiple instances of MSBuild the instance with the highest version will be used.*

**`--projects`**

Defines projects that should be analyzed.

**`-p|--properties`** `<NAME=VALUE>`

Defines one or more MSBuild properties.

**`--use-roslynator-analyzers`**

Indicates whether code analysis should use analyzers from nuget package [Roslynator.Analyzers](https://nuget.org/packages/Roslynator.Analyzers).

**`-v|--verbosity`** `{q[uiet]|m[inimal]|n[ormal]|d[etailed]}`

Defines the amount of information to display in the log.

## See Also

* [Roslynator Command-Line Interface](README.md)
