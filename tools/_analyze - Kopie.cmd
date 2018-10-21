@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

set _analyzersDir=..\src\Analyzers.CodeFixes\bin\Debug\netstandard1.3\

"..\src\CommandLine\bin\Debug\net461\roslynator" analyze "..\src\Analyzers.CodeFixes\Analyzers.CodeFixes.csproj" ^
 --analyzer-assemblies ^
  "%_analyzersDir%Roslynator.Common.dll" ^
  "%_analyzersDir%Roslynator.Common.Workspaces.dll" ^
  "%_analyzersDir%Roslynator.CSharp.Analyzers.CodeFixes.dll" ^
  "%_analyzersDir%Roslynator.CSharp.Analyzers.dll" ^
  "%_analyzersDir%Roslynator.CSharp.dll" ^
  "%_analyzersDir%Roslynator.CSharp.Workspaces.dll" ^
 --ignore-analyzer-references ^
 --ignored-diagnostics CS1591 ^
 --culture en

pause
