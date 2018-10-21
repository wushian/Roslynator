
# `lloc` Command

Counts logical lines of code in the specified project or solution.

## Synopsis

```
roslynator lloc <PROJECT|SOLUTION>
[--ignored-projects]
[--include-comments]
[--include-generated]
[--include-preprocessor-directives]
[--include-whitespace]
[--language]
[--msbuild-path]
[-p|--properties]
```

## Arguments

**`PROJECT|SOLUTION`**

The project or solution to calculate lines of code in.

### Optional Options

In C# block boundary is opening brace or closing brace.

In Visual Basic block boundary is end-block-statement such as `End Class`.

**`--ignored-projects`**

Defines project names that should not be fixed.

**`--include-comments`**

Indicates whether a line that contains only comment should be counted.

**`--include-generated`**

Indicates whether generated code should be counted.

**`--include-preprocessor-directives`**

Indicates whether preprocessor directive line should be counted.

**`--include-whitespace`**

Indicates whether white-space line should be counted.

**`--language`** {csharp|vb}

Defines project language.

**`--msbuild-path`**

Defines a path to MSBuild.

*Note: If the path to MSBuild is not specified and there are installed multiple instances of MSBuild the instance with the highest version will be used.*

**`-p|--properties`** `<NAME=VALUE>`

Defines one or more MSBuild properties.

## See Also

* [Roslynator Command-Line Interface](README.md)
