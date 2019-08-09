@echo off

cd ..\src\VisualStudioCode\package

del /S /Q roslyn\*.dll

cd roslyn

copy /Y ..\..\..\VisualStudio\bin\Release\Roslynator.Core.dll common
copy /Y ..\..\..\VisualStudio\bin\Release\Roslynator.Common.dll common
copy /Y ..\..\..\VisualStudio\bin\Release\Roslynator.CSharp.dll common
copy /Y ..\..\..\VisualStudio\bin\Release\Roslynator.Workspaces.Core.dll common
copy /Y ..\..\..\VisualStudio\bin\Release\Roslynator.Workspaces.Common.dll common
copy /Y ..\..\..\VisualStudio\bin\Release\Roslynator.CSharp.Workspaces.dll common

copy /Y ..\..\..\VisualStudio\bin\Release\Roslynator.CSharp.Analyzers.dll analyzers
copy /Y ..\..\..\VisualStudio\bin\Release\Roslynator.CSharp.Analyzers.CodeFixes.dll analyzers

copy /Y ..\..\..\VisualStudio\bin\Release\Roslynator.CSharp.Refactorings.dll refactorings

copy /Y ..\..\..\VisualStudio\bin\Release\Roslynator.CSharp.CodeFixes.dll fixes

cd ..

echo Package is being created
vsce package
echo Package successfully created

del /S /Q roslyn

echo OK

pause
