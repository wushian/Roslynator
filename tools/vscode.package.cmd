@echo off

cd ..\src\VisualStudioCode\package

del /S /Q roslyn

copy /Y ..\..\VisualStudio\bin\Release\Roslynator.Core.dll roslyn\common
copy /Y ..\..\VisualStudio\bin\Release\Roslynator.Common.dll roslyn\common
copy /Y ..\..\VisualStudio\bin\Release\Roslynator.CSharp.dll roslyn\common
copy /Y ..\..\VisualStudio\bin\Release\Roslynator.Workspaces.Core.dll roslyn\common
copy /Y ..\..\VisualStudio\bin\Release\Roslynator.Workspaces.Common.dll roslyn\common
copy /Y ..\..\VisualStudio\bin\Release\Roslynator.CSharp.Workspaces.dll roslyn\common
copy /Y ..\..\VisualStudio\bin\Release\Roslynator.CSharp.Analyzers.dll roslyn\analyzers
copy /Y ..\..\VisualStudio\bin\Release\Roslynator.CSharp.Analyzers.CodeFixes.dll roslyn\analyzers
copy /Y ..\..\VisualStudio\bin\Release\Roslynator.CSharp.Refactorings.dll roslyn\refactorings
copy /Y ..\..\VisualStudio\bin\Release\Roslynator.CSharp.CodeFixes.dll roslyn\fixes

vsce package

del /S /Q roslyn

echo OK
pause
