# Roslynator for Visual Studio Code

* A collection of 500+ [analyzers](https://github.com/JosefPihrt/Roslynator/blob/master/src/Analyzers/README.md), [refactorings](https://github.com/JosefPihrt/Roslynator/blob/master/src/Refactorings/README.md) and [fixes](https://github.com/JosefPihrt/Roslynator/blob/master/src/CodeFixes/README.md) for C#, powered by [Roslyn](https://github.com/dotnet/roslyn).

* For further information please with Roslynator [repo](https://github.com/JosefPihrt/Roslynator).

* This extension requires [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp) 1.19.0 or higher.

## Configuration

To make this extension working it is necessary to add following configuration to `%USERPROFILE%\.omnisharp\omnisharp.json`:

```json
{
    "RoslynExtensionsOptions": {
        "EnableAnalyzersSupport": true,
        "LocationPaths": [
            "%USERPROFILE%/.vscode/extensions/josefpihrt-vscode.roslynator-2.1.3/roslyn/common",
            "%USERPROFILE%/.vscode/extensions/josefpihrt-vscode.roslynator-2.1.3/roslyn/analyzers",
            "%USERPROFILE%/.vscode/extensions/josefpihrt-vscode.roslynator-2.1.3/roslyn/refactorings",
            "%USERPROFILE%/.vscode/extensions/josefpihrt-vscode.roslynator-2.1.3/roslyn/fixes"
        ]
    }
}
```

Replace `%USERPROFILE%` with an actual path (such as `C:/Users/User`).

After each update of the extension it is necessary to update paths to libraries (i.e. replace version in the path with a new version).

## Donation

Although Roslynator products are free of charge, any [donation](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BX85UA346VTN6) is welcome and supports further development.
