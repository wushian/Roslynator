
# `format` Command

Formats documents in the specified project or solution.

## Synopsis

```
roslynator format <PROJECT|SOLUTION>
[--include-generated]
[--ignored-projects]
[--language]
[--msbuild-path]
[-p|--properties]
```

## Arguments

**`PROJECT|SOLUTION`**

The project or solution to fix.

### Optional Options

**`--include-generated`**

Indicates whether generated code should be formatted.

**`--ignored-projects`**

Defines project names that should not be formatted.

**`--language`** {csharp|vb}

Defines project language.

**`--msbuild-path`**

Defines a path to MSBuild.

*Note: If the path to MSBuild is not specified and there are installed multiple instances of MSBuild the instance with the highest version will be used.*

**`-p|--properties`** `<NAME=VALUE>`

Defines one or more MSBuild properties.

## See Also

* [Roslynator Command-Line Interface](README.md)
