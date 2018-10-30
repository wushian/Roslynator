
# `sln` Command

Gets in information about solution and its projects.

## Synopsis

```
roslynator sln <PROJECT|SOLUTION>
[--ignored-projects]
[--language]
[--file-log]
[--file-log-verbosity]
[--msbuild-path]
[--projects]
[-p|--properties]
[-v|--verbosity]
```

## Arguments

**`PROJECT|SOLUTION`**

The project or solution to open.

### Optional Options

**`--ignored-projects`**

Defines project names that should not be fixed.

**`--language`** `{cs[harp]|v[isual-]b[asic])}`

Defines project language.

**`--msbuild-path`**

Defines a path to MSBuild.

*Note: If the path to MSBuild is not specified and there are installed multiple instances of MSBuild the instance with the highest version will be used.*

**`--projects`**

Defines projects that should be analyzed.

**`-p|--properties`** `<NAME=VALUE>`

Defines one or more MSBuild properties.

**`-v|--verbosity`** `{q[uiet]|m[inimal]|n[ormal]|d[etailed]|diag[nostic]}`

Defines the amount of information to display in the log.

## See Also

* [Roslynator Command-Line Interface](README.md)
