
# `analyze` Command

Analyzes specified project or solution and reports diagnostics.

## Synopsis

```
roslynator analyze <PROJECT|SOLUTION>
[-a|--analyzer-assemblies]
[--culture]
[--execution-time]
[--ignore-analyzer-references]
[--ignore-compiler-diagnostics]
[--ignored-diagnostics]
[--ignored-projects]
[--language]
[--file-log]
[--file-log-verbosity]
[--minimal-severity]
[--msbuild-path]
[--projects]
[-p|--properties]
[--report-fade-diagnostics]
[--report-suppressed-diagnostics]
[--supported-diagnostics]
[--use-roslynator-analyzers]
[-v|--verbosity]
[--xml-file-log]
```

## Arguments

**`PROJECT|SOLUTION`**

The project or solution to analyze.

### Optional Options

**`a-|--analyzer-assemblies`**

Defines one or more paths to:

* analyzer assembly
* directory that should be searched recursively for analyzer assemblies

**`--culture`**

Defines culture that should be used to display diagnostic message.

**`--execution-time`**

Indicates whether to measure execution time of each analyzer.

**`--ignore-analyzer-references`**

Indicates whether Roslynator should ignore analyzers that are referenced in projects.

**`--ignore-compiler-diagnostics`**

Indicates whether to display compiler diagnostics.

**`--ignored-diagnostics`**

Defines diagnostic identifiers that should not be reported.

**`--ignored-projects`**

Defines projects that should not be analyzed.

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

**`--report-fade-diagnostics`**

Indicates whether diagnostics whose ID ends with 'FadedToken' or 'FadeOut' should be reported.

**`--report-suppressed-diagnostics`**

Indicates whether suppressed diagnostics should be reported.

**`--supported-diagnostics`**

Defines diagnostic identifiers that should be reported.

**`--use-roslynator-analyzers`**

Indicates whether code analysis should use analyzers from nuget package [Roslynator.Analyzers](https://nuget.org/packages/Roslynator.Analyzers).

**`-v|--verbosity`** `{q[uiet]|m[inimal]|n[ormal]|d[etailed]|diag[nostic]}`

Defines the amount of information to display in the log.

**`--xml-file-log`**

Defines path to file that will store reported diagnostics in XML format.

## See Also

* [Roslynator Command-Line Interface](README.md)
