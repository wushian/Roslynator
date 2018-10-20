
# `format` Command

Formats documents in the specified solution.

## Synopsis

```
roslynator format <SOLUTION>
[--ignored-projects]
[--msbuild-path]
[-p|--properties]
```

## Arguments

**`SOLUTION`**

The solution file to fix.

### Optional Options

**`--ignored-projects`**

Defines project names that should not be formatted.

**`--msbuild-path`**

Defines a path to MSBuild.

*Note: If the path to MSBuild is not specified and there are installed multiple instances of MSBuild the instance with the highest version will be used.*

**`-p|--properties`** `<NAME=VALUE>`

Defines one or more MSBuild properties.

## See Also

* [Roslynator Command-Line Interface](README.md)
